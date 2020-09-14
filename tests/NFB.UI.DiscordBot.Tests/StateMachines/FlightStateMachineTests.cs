namespace NFB.UI.DiscordBot.Tests.StateMachines
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Discord.WebSocket;

    using MassTransit.Testing;

    using Moq;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.StateMachines;
    using NFB.UI.DiscordBot.States;
    using NFB.UI.DiscordBot.Tests.TestFactory;

    using Xunit;

    /// <summary>
    /// The flight state machine tests.
    /// </summary>
    public class FlightStateMachineTests
    {
        #region Public Methods

        /// <summary>
        /// Receive flight created event new saga in created state.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ReceiveFlightCreatedEventNewSagaInCreatedState()
        {
            var mockDiscord = new Mock<DiscordBotTestSocketClient>();
            mockDiscord.SetupGet(p => p.Guilds).Returns(new List<SocketGuild>(new SocketGuild[1]));

            var harness = new DiscordBotTestBus();
            var stateMachine = new FlightStateMachine(mockDiscord.Object);
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);
            var id = Guid.NewGuid();

            await harness.Start();

            await harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            Assert.NotNull(await saga.Exists(id, stateMachine.Created, TimeSpan.FromSeconds(1)));
            Assert.Null(await saga.Exists(id, stateMachine.Active, TimeSpan.FromSeconds(1)));

            await harness.Stop();
        }

        /// <summary>
        /// Receive flight starting event saga in active state.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ReceiveFlightStartingEventSagaInActiveState()
        {
            var mockDiscord = new Mock<DiscordBotTestSocketClient>();
            mockDiscord.SetupGet(p => p.Guilds).Returns(new List<SocketGuild>(new SocketGuild[1]));

            var harness = new DiscordBotTestBus();
            var stateMachine = new FlightStateMachine(mockDiscord.Object);
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);
            var id = Guid.NewGuid();

            await harness.Start();

            await harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            Assert.NotNull(await saga.Exists(id, stateMachine.Created, TimeSpan.FromSeconds(1)));
            Assert.Null(await saga.Exists(id, stateMachine.Active, TimeSpan.FromSeconds(1)));

            await harness.Bus.Publish(new FlightStartingEvent
            {
                Id = id,
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            Assert.NotNull(await saga.Exists(id, stateMachine.Active, TimeSpan.FromSeconds(1)));

            await harness.Stop();
        }

        #endregion Public Methods
    }
}