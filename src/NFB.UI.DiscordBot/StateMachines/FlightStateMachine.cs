﻿namespace NFB.UI.DiscordBot.StateMachines
{
    using System;

    using Automatonymous;

    using MassTransit;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Activities;
    using NFB.UI.DiscordBot.Events;
    using NFB.UI.DiscordBot.Extensions;
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

            // Instance
            this.InstanceState(p => p.CurrentState);

            // Events
            this.Event(() => this.FlightCreatedEvent, x => x.CorrelateById(p => p.Message.Id));
            this.Event(() => this.FlightStartingEvent, x => x.CorrelateById(p => p.Message.Id));
            this.Event(() => this.UserJoinedVoiceChannelEvent, x => x.CorrelateBy(p => p.VoiceChannelId, p => p.Message.ChannelId.ToGuid()));
            this.Event(() => this.UserLeftVoiceChannelEvent, x => x.CorrelateBy(p => p.VoiceChannelId, p => p.Message.ChannelId.ToGuid()));
            this.Event(() => this.VatsimPilotUpdatedEvent, x => x.CorrelateBy((state, context) => state.UsersInVoiceChannel.Contains(context.Message.UserId.ToGuid())));

            // Schedules
            this.Schedule(() => this.UpdatePilotDataInMessage, p => p.UpdatePilotDataInMessageToken, s => s.Received = p => p.CorrelateById(m => m.Message.Id));

            // Work flow
            this.Initially(
                this.When(this.FlightCreatedEvent)
                    .Activity(x => x.OfType<CreateDiscordChannelActivity>())
                    .TransitionTo(this.Created),
                this.When(this.FlightStartingEvent)
                    .TransitionTo(this.Created));

            this.During(
                this.Created,
                this.When(this.FlightStartingEvent)
                    .Activity(x => x.OfType<CreateVoiceChannelActivity>())
                    .TransitionTo(this.Active),
                this.When(this.FlightCreatedEvent)
                    .Activity(x => x.OfType<CreateDiscordChannelActivity>())
                    .Activity(x => x.OfType<CreateVoiceChannelActivity>())
                    .TransitionTo(this.Active));

            this.During(
                this.Active,
                this.When(this.UserJoinedVoiceChannelEvent)
                    .Then(context => context.Instance.UsersInVoiceChannel.Add(context.Data.UserId.ToGuid()))
                    .Activity(x => x.OfType<UpdateActiveFlightMessageActivity>())
                    .Schedule(
                        this.UpdatePilotDataInMessage,
                        context => context.Init<UpdatePilotDataInMessage>(new UpdatePilotDataInMessage
                        {
                            Id = context.Instance.CorrelationId
                        }),
                        context => TimeSpan.FromSeconds(5)),
                this.When(this.UserLeftVoiceChannelEvent)
                    .Activity(x => x.OfType<UpdateActiveFlightMessageActivity>())
                    .Schedule(
                        this.UpdatePilotDataInMessage,
                        context => context.Init<UpdatePilotDataInMessage>(new UpdatePilotDataInMessage
                        {
                            Id = context.Instance.CorrelationId
                        }),
                        context => TimeSpan.FromSeconds(5)),
                this.When(this.VatsimPilotUpdatedEvent)
                    .Activity(x => x.OfType<UpdateVatsimPilotDataActivity>())
                    .Schedule(
                        this.UpdatePilotDataInMessage,
                        context => context.Init<UpdatePilotDataInMessage>(new UpdatePilotDataInMessage
                        {
                            Id = context.Instance.CorrelationId
                        }),
                        context => TimeSpan.FromSeconds(5)),
                this.When(this.UpdatePilotDataInMessage.Received)
                    .Activity(x => x.OfType<UpdateActiveFlightMessageActivity>()));
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the active.
        /// </summary>
        public State Active { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public State Created { get; set; }

        /// <summary>
        /// Gets or sets the flight created event.
        /// </summary>
        public Event<FlightCreatedEvent> FlightCreatedEvent { get; set; }

        /// <summary>
        /// Gets or sets the flight starting event.
        /// </summary>
        public Event<FlightStartingEvent> FlightStartingEvent { get; set; }

        /// <summary>
        /// Gets or sets the update pilot data in message.
        /// </summary>
        public Schedule<FlightState, UpdatePilotDataInMessage> UpdatePilotDataInMessage { get; set; }

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

        #endregion Public Properties
    }
}