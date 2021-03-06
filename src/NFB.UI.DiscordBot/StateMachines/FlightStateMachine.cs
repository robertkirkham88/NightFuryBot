﻿namespace NFB.UI.DiscordBot.StateMachines
{
    using System;
    using System.Threading;

    using Automatonymous;

    using MassTransit;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Schedules;
    using NFB.UI.DiscordBot.StateMachines.Activities;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The flight state machine.
    /// </summary>
    public class FlightStateMachine : MassTransitStateMachine<FlightState>
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightStateMachine"/> class.
        /// </summary>
        public FlightStateMachine()
        {
            // Initialize
            this.SetCompletedWhenFinalized();

            // Instance
            this.InstanceState(p => p.CurrentState);

            // Events
            this.Event(() => this.FlightSubmittedEvent, x => x.CorrelateById(p => p.Message.Id));
            this.Event(() => this.FlightInvalidEvent, x => x.CorrelateById(p => p.Message.Id));
            this.Event(() => this.FlightCreatedEvent, x => x.CorrelateById(p => p.Message.Id));
            this.Event(() => this.FlightCompletedEvent, x => x.CorrelateById(p => p.Message.Id));
            this.Event(() => this.FlightStartingEvent, x => x.CorrelateById(p => p.Message.Id));
            this.Event(() => this.UserJoinedVoiceChannelEvent, x => x.CorrelateBy<ulong>(p => p.VoiceChannelId, p => p.Message.ChannelId));
            this.Event(() => this.UserLeftVoiceChannelEvent, x => x.CorrelateBy<ulong>(p => p.VoiceChannelId, p => p.Message.ChannelId));
            this.Event(() => this.VatsimPilotUpdatedEvent, x => x.CorrelateBy((state, context) => state.UsersInVoiceChannel.Contains(context.Message.UserId)));
            this.Event(() => this.VoiceChannelRemovedEvent, x => x.CorrelateBy<ulong>(p => p.VoiceChannelId, p => p.Message.Id));

            // Schedules
            this.Schedule(() => this.UpdatePilotDataSchedule, p => p.UpdatePilotDataInMessageToken, s => s.Received = p => p.CorrelateById(m => m.Message.Id));
            this.Schedule(() => this.CheckFlightCompletedSchedule, p => p.CheckFlightCompletedToken, s => s.Received = p => p.CorrelateById(m => m.Message.Id));
            this.Schedule(() => this.UpdateVoiceChannelUsersSchedule, p => p.UpdateVoiceChannelUsersToken, s => s.Received = p => p.CorrelateById(m => m.Message.Id));
            this.Schedule(() => this.RequestVatsimUpdateSchedule, p => p.RequestVatsimUpdateScheduleToken, s => s.Received = p => p.CorrelateById(m => m.Message.Id));

            // Work flow
            this.Initially(
                this.When(this.FlightSubmittedEvent)
                    .Then(
                        (context) =>
                            {
                                context.Instance.ChannelData = context.Data.ChannelData;
                                context.Instance.RequestMessageId = context.Data.RequestMessageId;
                            })
                .TransitionTo(this.Submitted));

            this.During(
                this.Submitted,
                this.When(this.FlightCreatedEvent)
                    .Activity(x => x.OfType<CreateAnnouncementMessageActivity>())
                    .TransitionTo(this.Created),
                this.When(this.FlightInvalidEvent)
                    .Activity(x => x.OfType<FlightSubmissionFailedActivity>())
                    .Finalize(),
                this.When(this.FlightStartingEvent)
                    .TransitionTo(this.Created));

            this.During(
                this.Created,
                this.When(this.FlightStartingEvent)
                    .Activity(x => x.OfType<CreateVoiceChannelActivity>())
                    .Activity(x => x.OfType<CreateActiveFlightMessageActivity>())
                    .Activity(x => x.OfType<DeleteAnnouncementMessageActivity>())
                    .Schedule(
                        this.CheckFlightCompletedSchedule,
                        context => context.Init<CheckFlightCompletedScheduleMessage>(
                            new CheckFlightCompletedScheduleMessage { Id = context.Instance.CorrelationId }),
                        context => context.Instance.StartTime.AddHours(1))
                    .Schedule(
                        this.UpdateVoiceChannelUsersSchedule,
                        context => context.Init<UpdateVoiceChannelUsersScheduleMessage>(
                            new UpdateVoiceChannelUsersScheduleMessage { Id = context.Instance.CorrelationId }),
                        context => TimeSpan.FromMinutes(15))
                    .TransitionTo(this.Active),
                this.When(this.FlightCreatedEvent)
                    .Activity(x => x.OfType<CreateVoiceChannelActivity>())
                    .Activity(x => x.OfType<CreateActiveFlightMessageActivity>())
                    .Schedule(
                        this.CheckFlightCompletedSchedule,
                        context => context.Init<CheckFlightCompletedScheduleMessage>(
                            new CheckFlightCompletedScheduleMessage { Id = context.Instance.CorrelationId }),
                        context => context.Instance.StartTime.AddHours(1))
                    .Schedule(
                        this.UpdateVoiceChannelUsersSchedule,
                        context => context.Init<UpdateVoiceChannelUsersScheduleMessage>(
                            new UpdateVoiceChannelUsersScheduleMessage { Id = context.Instance.CorrelationId }),
                        context => TimeSpan.FromMinutes(15))
                    .TransitionTo(this.Active));

            this.During(
                this.Active,
                this.When(this.UserJoinedVoiceChannelEvent)
                    .Activity(x => x.OfType<AddUserToFlightActivity>())
                    .Activity(x => x.OfType<UpdateActiveFlightMessageActivity>())
                    .If(
                        (context) => context.Instance.RequestVatsimUpdateScheduleToken.HasValue == false,
                        (activity) => activity.Schedule(
                            this.RequestVatsimUpdateSchedule,
                            context => context.Init<RequestVatsimUpdateScheduleMessage>(new RequestVatsimUpdateScheduleMessage { Id = context.Instance.CorrelationId }),
                            context => TimeSpan.FromMinutes(2)))
                    .Schedule(
                        this.UpdatePilotDataSchedule,
                        context => context.Init<UpdatePilotDataScheduleMessage>(
                            new UpdatePilotDataScheduleMessage { Id = context.Instance.CorrelationId }),
                        context => TimeSpan.FromSeconds(5))
                    .Unschedule(this.CheckFlightCompletedSchedule),
                this.When(this.UserLeftVoiceChannelEvent)
                    .Activity(x => x.OfType<RemoveUserFromFlightActivity>())
                    .Activity(x => x.OfType<UpdateActiveFlightMessageActivity>())
                    .If(
                        (context) => context.Instance.UsersInVoiceChannel.Count == 0,
                        (activity) => activity.Unschedule(this.RequestVatsimUpdateSchedule))
                    .Schedule(
                        this.UpdatePilotDataSchedule,
                        context => context.Init<UpdatePilotDataScheduleMessage>(
                            new UpdatePilotDataScheduleMessage { Id = context.Instance.CorrelationId }),
                        context => TimeSpan.FromSeconds(5))
                    .Schedule(
                        this.CheckFlightCompletedSchedule,
                        context => context.Init<CheckFlightCompletedScheduleMessage>(
                            new CheckFlightCompletedScheduleMessage { Id = context.Instance.CorrelationId }),
                        context => TimeSpan.FromHours(1)),
                this.When(this.VatsimPilotUpdatedEvent)
                    .Activity(x => x.OfType<UpdateVatsimPilotDataActivity>())
                    .Unschedule(this.UpdatePilotDataSchedule)
                    .Schedule(
                        this.UpdatePilotDataSchedule,
                        context => context.Init<UpdatePilotDataScheduleMessage>(
                            new UpdatePilotDataScheduleMessage { Id = context.Instance.CorrelationId }),
                        context => TimeSpan.FromSeconds(5)),
                this.When(this.UpdatePilotDataSchedule.Received)
                    .Activity(x => x.OfType<UpdateActiveFlightMessageActivity>()),
                this.When(this.CheckFlightCompletedSchedule.Received)
                    .Activity(x => x.OfType<CheckFlightCompletedActivity>()),
                this.When(this.UpdateVoiceChannelUsersSchedule.Received)
                    .Activity(x => x.OfType<UpdateVoiceChannelUsersActivity>())
                    .Schedule(
                        this.UpdateVoiceChannelUsersSchedule,
                        context => context.Init<UpdateVoiceChannelUsersScheduleMessage>(
                            new UpdateVoiceChannelUsersScheduleMessage { Id = context.Instance.CorrelationId }),
                        context => TimeSpan.FromMinutes(15)),
                this.When(this.RequestVatsimUpdateSchedule.Received)
                    .Publish(new RequestVatsimPilotDataEvent())
                    .If(
                        (context) => context.Instance.UsersInVoiceChannel.Count > 0,
                        (activity) => activity.Schedule(
                            this.RequestVatsimUpdateSchedule,
                            context => context.Init<RequestVatsimUpdateScheduleMessage>(new RequestVatsimUpdateScheduleMessage { Id = context.Instance.CorrelationId }),
                            context => TimeSpan.FromMinutes(2))),
                this.When(this.VoiceChannelRemovedEvent)
                    .Publish((context) => new FlightCompletedEvent { Id = context.Instance.CorrelationId }),
                this.When(this.FlightCompletedEvent)
                    .Activity(x => x.OfType<DeleteActiveFlightMessageActivity>())
                    .Activity(x => x.OfType<DeleteVoiceChannelActivity>())
                    .Then(
                        context =>
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(3));
                            })
                    .Finalize());

            this.During(
                this.Final,
                this.Ignore(this.UpdateVoiceChannelUsersSchedule.Received),
                this.Ignore(this.CheckFlightCompletedSchedule.Received),
                this.Ignore(this.UpdatePilotDataSchedule.Received),
                this.Ignore(this.VoiceChannelRemovedEvent),
                this.Ignore(this.VatsimPilotUpdatedEvent),
                this.Ignore(this.FlightCompletedEvent));
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the active.
        /// </summary>
        public State Active { get; set; }

        /// <summary>
        /// Gets or sets the update voice channel users.
        /// </summary>
        public Schedule<FlightState, CheckFlightCompletedScheduleMessage> CheckFlightCompletedSchedule { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public State Created { get; set; }

        /// <summary>
        /// Gets or sets the flight completed event.
        /// </summary>
        public Event<FlightCompletedEvent> FlightCompletedEvent { get; set; }

        /// <summary>
        /// Gets or sets the flight created event.
        /// </summary>
        public Event<FlightCreatedEvent> FlightCreatedEvent { get; set; }

        /// <summary>
        /// Gets or sets the flight invalid event.
        /// </summary>
        public Event<FlightInvalidEvent> FlightInvalidEvent { get; set; }

        /// <summary>
        /// Gets or sets the flight starting event.
        /// </summary>
        public Event<FlightStartingEvent> FlightStartingEvent { get; set; }

        /// <summary>
        /// Gets or sets the flight submitted event.
        /// </summary>
        public Event<FlightSubmittedEvent> FlightSubmittedEvent { get; set; }

        /// <summary>
        /// Gets or sets the request vatsim update schedule.
        /// </summary>
        public Schedule<FlightState, RequestVatsimUpdateScheduleMessage> RequestVatsimUpdateSchedule { get; set; }

        /// <summary>
        /// Gets or sets the submitted.
        /// </summary>
        public State Submitted { get; set; }

        /// <summary>
        /// Gets or sets the update pilot data in message.
        /// </summary>
        public Schedule<FlightState, UpdatePilotDataScheduleMessage> UpdatePilotDataSchedule { get; set; }

        /// <summary>
        /// Gets or sets the update voice channel users schedule.
        /// </summary>
        public Schedule<FlightState, UpdateVoiceChannelUsersScheduleMessage> UpdateVoiceChannelUsersSchedule { get; set; }

        /// <summary>
        /// Gets or sets the user joined voice channel event.
        /// </summary>
        public Event<UserJoinedVoiceChannelEvent> UserJoinedVoiceChannelEvent { get; set; }

        /// <summary>
        /// Gets or sets the user left voice channel event.
        /// </summary>
        public Event<UserLeftVoiceChannelEvent> UserLeftVoiceChannelEvent { get; set; }

        /// <summary>
        /// Gets or sets the vatsim pilot updated event.
        /// </summary>
        public Event<VatsimPilotUpdatedEvent> VatsimPilotUpdatedEvent { get; set; }

        /// <summary>
        /// Gets or sets the voice channel removed event.
        /// </summary>
        public Event<VoiceChannelRemovedEvent> VoiceChannelRemovedEvent { get; set; }

        #endregion Public Properties
    }
}