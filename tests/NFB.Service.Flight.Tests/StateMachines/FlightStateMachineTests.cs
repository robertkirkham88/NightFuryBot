namespace NFB.Service.Flight.Tests.StateMachines
{
    using System;
    using System.Threading.Tasks;

    using MassTransit.Testing;

    using NFB.Domain.Bus.Events;
    using NFB.Service.Flight.StateMachines;
    using NFB.Service.Flight.States;
    using NFB.Service.Flight.Tests.TestFactory;

    using Xunit;

    /// <summary>
    /// The flight state machine tests.
    /// </summary>
    public class FlightStateMachineTests
    {
        #region Public Methods

        /// <summary>
        /// Receive flight created event new saga in created state task.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ReceiveFlightCreatedEventNewSagaInCreatedState()
        {
            var harness = new FlightServiceTestBus();
            var stateMachine = new FlightStateMachine();
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);

            await harness.Start();

            var id = Guid.NewGuid();

            await harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            var instance = await saga.Exists(id, stateMachine.Created, TimeSpan.FromSeconds(5));
            Assert.NotNull(instance);

            await harness.Stop();
        }

        /// <summary>
        /// Receive flight created event within 15 minute start time saga in starting state.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ReceiveFlightCreatedEventWithin15MinuteStartTimeSagaInStartingState()
        {
            var harness = new FlightServiceTestBus();
            var stateMachine = new FlightStateMachine();
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);

            await harness.Start();

            var id = Guid.NewGuid();

            await harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddMinutes(10)
            });

            var instance = await saga.Exists(id, stateMachine.Starting, TimeSpan.FromSeconds(5));
            Assert.NotNull(instance);

            await harness.Stop();
        }

        /// <summary>
        /// Receive flight created event with date time in a few seconds
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ReceiveFlightCreatedEventWithStartTimeWithinSecondsShouldBeStarted()
        {
            var harness = new FlightServiceTestBus();
            var stateMachine = new FlightStateMachine();
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);

            await harness.Start();

            var id = Guid.NewGuid();

            await harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddSeconds(1)
            });

            var instance = await saga.Exists(id, stateMachine.Started, TimeSpan.FromSeconds(10));
            Assert.NotNull(instance);

            await harness.Stop();
        }

        /// <summary>
        /// Started flight publishes event
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task StartedFlightPublishesEvent()
        {
            var harness = new FlightServiceTestBus();
            var stateMachine = new FlightStateMachine();
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);

            await harness.Start();

            var id = Guid.NewGuid();

            await harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddSeconds(5)
            });

            var instance = await saga.Exists(id, stateMachine.Started, TimeSpan.FromSeconds(5));
            Assert.NotNull(instance);

            Assert.True(await harness.Published.Any<FlightStartedEvent>(p => p.Context.Message.Id == id));

            await harness.Stop();
        }

        /// <summary>
        /// Starting flight publishes an event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task StartingFlightPublishesEvent()
        {
            var harness = new FlightServiceTestBus();
            var stateMachine = new FlightStateMachine();
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);

            await harness.Start();

            var id = Guid.NewGuid();

            await harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddMinutes(10)
            });

            var instance = await saga.Exists(id, stateMachine.Starting, TimeSpan.FromSeconds(5));
            Assert.NotNull(instance);

            Assert.True(await harness.Published.Any<FlightStartingEvent>(p => p.Context.Message.Id == id));

            await harness.Stop();
        }

        #endregion Public Methods
    }
}