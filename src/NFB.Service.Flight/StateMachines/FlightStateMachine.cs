namespace NFB.Service.Flight.StateMachines
{
    using System;

    using Automatonymous;

    using MassTransit;

    using NFB.Domain.Bus.Events;
    using NFB.Service.Flight.Events;
    using NFB.Service.Flight.States;

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
            // State
            this.InstanceState(p => p.CurrentState);

            // Events
            this.Event(() => this.FlightCreatedEvent, x => x.CorrelateById(p => p.Message.Id));

            // Schedules
            this.Schedule(
                () => this.FlightStartingSchedule,
                x => x.FlightStartingScheduleToken,
                s => { s.Received = p => p.CorrelateById(m => m.Message.Id); });

            this.Schedule(
                () => this.FlightStartedSchedule,
                x => x.FlightStartedScheduledToken,
                s => { s.Received = p => p.CorrelateById(m => m.Message.Id); });

            // Work flow
            this.Initially(
                this.When(this.FlightCreatedEvent)
                    .TransitionTo(this.Created)
                    .Schedule(
                        this.FlightStartingSchedule,
                        context => context.Init<FlightStarting>(
                            new FlightStarting
                            {
                                Id = context.Data.Id,
                                Origin = context.Data.Origin,
                                Destination = context.Data.Destination,
                                StartTime = context.Data.StartTime
                            }),
                        context =>
                            {
                                var startingTime = context.Data.StartTime.Add(TimeSpan.FromMinutes(-15));

                                return startingTime <= DateTime.UtcNow ? DateTime.UtcNow.AddSeconds(3) : startingTime; // Slight delay due to message ordering.
                            }));

            this.During(
                this.Created,
                this.When(this.FlightStartingSchedule.Received)
                    .TransitionTo(this.Starting)
                    .Publish(context =>
                        {
                            var @event = new FlightStartingEvent
                            {
                                Id = context.Data.Id,
                                Origin = context.Data.Origin,
                                Destination = context.Data.Destination,
                                StartTime = context.Data.StartTime
                            };
                            return @event;
                        })
                    .Schedule(
                        this.FlightStartedSchedule,
                        context => context.Init<FlightStarted>(new FlightStarted { Id = context.Data.Id }),
                        context =>
                            {
                                var startTime = context.Data.StartTime;

                                return startTime <= DateTime.UtcNow ? DateTime.UtcNow.AddSeconds(3) : startTime; // Slight delay due to message ordering.
                            }));

            this.During(
                this.Starting,
                this.When(this.FlightStartedSchedule.Received)
                    .Publish(context =>
                        {
                            var @event = new FlightStartedEvent
                            {
                                Id = context.Data.Id,
                            };
                            return @event;
                        })
                    .TransitionTo(this.Started));
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public State Created { get; set; }

        /// <summary>
        /// Gets or sets the flight created event.
        /// </summary>
        public Event<FlightCreatedEvent> FlightCreatedEvent { get; set; }

        /// <summary>
        /// Gets or sets the flight started schedule.
        /// </summary>
        public Schedule<FlightState, FlightStarted> FlightStartedSchedule { get; set; }

        /// <summary>
        /// Gets or sets the flight starting schedule.
        /// </summary>
        public Schedule<FlightState, FlightStarting> FlightStartingSchedule { get; set; }

        /// <summary>
        /// Gets or sets the started.
        /// </summary>
        public State Started { get; set; }

        /// <summary>
        /// Gets or sets the starting.
        /// </summary>
        public State Starting { get; set; }

        #endregion Public Properties
    }
}