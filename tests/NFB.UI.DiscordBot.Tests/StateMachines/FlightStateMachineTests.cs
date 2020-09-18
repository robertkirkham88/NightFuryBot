namespace NFB.UI.DiscordBot.Tests.StateMachines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Discord.WebSocket;

    using MassTransit.Testing;

    using Moq;

    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Extensions;
    using NFB.UI.DiscordBot.StateMachines;
    using NFB.UI.DiscordBot.States;
    using NFB.UI.DiscordBot.Tests.TestFactory;

    using Xunit;

    /// <summary>
    /// The flight state machine tests.
    /// TODO: Cleanup tests as these are messy.
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
            var stateMachine = new FlightStateMachine();
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);
            var id = Guid.NewGuid();

            await harness.Start();

            await harness.Bus.Publish(
                new FlightCreatedEvent
                {
                    Id = id,
                    Destination = new AirportEntityDto { ICAO = "EGCC" },
                    Origin = new AirportEntityDto { ICAO = "EGLL" },
                    StartTime = DateTime.UtcNow.AddHours(3)
                });

            Assert.NotNull(await saga.Exists(id, stateMachine.Created, TimeSpan.FromSeconds(1)));
            Assert.Null(await saga.Exists(id, stateMachine.Active, TimeSpan.FromSeconds(1)));

            await harness.Stop();
        }

        /// <summary>
        /// Receive flight created event sets instance information.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ReceiveFlightCreatedEventSetsInstanceInformation()
        {
            var mockDiscord = new Mock<DiscordBotTestSocketClient>();
            mockDiscord.SetupGet(p => p.Guilds).Returns(new List<SocketGuild>(new SocketGuild[1]));

            var harness = new DiscordBotTestBus();
            var stateMachine = new FlightStateMachine();
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);
            var id = Guid.NewGuid();

            await harness.Start();

            await harness.Bus.Publish(
                new FlightCreatedEvent
                {
                    Id = id,
                    Destination = new AirportEntityDto { ICAO = "EGCC" },
                    Origin = new AirportEntityDto { ICAO = "EGLL" },
                    StartTime = DateTime.UtcNow.AddHours(3)
                });

            Assert.NotNull(await saga.Exists(id, stateMachine.Created, TimeSpan.FromSeconds(1)));

            var instance = saga.Created.Select(p => p.CorrelationId == id).FirstOrDefault();
            Assert.Equal("EGCC", instance?.Saga.Destination.ICAO);
            Assert.Equal("EGLL", instance?.Saga.Origin.ICAO);
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
            var stateMachine = new FlightStateMachine();
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);
            var id = Guid.NewGuid();

            await harness.Start();

            await harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = new AirportEntityDto { ICAO = "EGCC" },
                Origin = new AirportEntityDto { ICAO = "EGLL" },
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

        /// <summary>
        /// The user joined channel event adds user to user list.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task UserJoinedChannelEventAddsUserToUserList()
        {
            var mockDiscord = new Mock<DiscordBotTestSocketClient>();
            mockDiscord.SetupGet(p => p.Guilds).Returns(new List<SocketGuild>(new SocketGuild[1]));

            var harness = new DiscordBotTestBus();
            var stateMachine = new FlightStateMachine();
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);
            var id = Guid.NewGuid();

            await harness.Start();

            await harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = new AirportEntityDto { ICAO = "EGCC" },
                Origin = new AirportEntityDto { ICAO = "EGLL" },
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            Assert.NotNull(await saga.Exists(id, stateMachine.Created, TimeSpan.FromSeconds(1)));
            Assert.Null(await saga.Exists(id, stateMachine.Active, TimeSpan.FromSeconds(1)));

            await harness.Bus.Publish(new FlightStartingEvent
            {
                Id = id,
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            await saga.Exists(id, stateMachine.Active, TimeSpan.FromSeconds(1));

            var sagaInstance = saga.Created.Select(p => p.CorrelationId == id).First();
            sagaInstance.Saga.VoiceChannelId = ((ulong)123456789).ToGuid();

            await harness.Bus.Publish(new UserJoinedVoiceChannelEvent { ChannelId = 123456789, UserId = 1111111111 });
            Thread.Sleep(500);
            Assert.Single(sagaInstance.Saga.UsersInVoiceChannel.Where(p => p == ((ulong)1111111111).ToGuid()));
        }

        /// <summary>
        /// The user left channel event removes user from user list.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task UserLeftChannelEventRemovesUserFromUserList()
        {
            var mockDiscord = new Mock<DiscordBotTestSocketClient>();
            mockDiscord.SetupGet(p => p.Guilds).Returns(new List<SocketGuild>(new SocketGuild[1]));

            var harness = new DiscordBotTestBus();
            var stateMachine = new FlightStateMachine();
            var saga = harness.StateMachineSaga<FlightState, FlightStateMachine>(stateMachine);
            var id = Guid.NewGuid();

            await harness.Start();

            await harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = new AirportEntityDto { ICAO = "EGCC" },
                Origin = new AirportEntityDto { ICAO = "EGLL" },
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            Assert.NotNull(await saga.Exists(id, stateMachine.Created, TimeSpan.FromSeconds(1)));
            Assert.Null(await saga.Exists(id, stateMachine.Active, TimeSpan.FromSeconds(1)));

            await harness.Bus.Publish(new FlightStartingEvent
            {
                Id = id,
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            await saga.Exists(id, stateMachine.Active, TimeSpan.FromSeconds(1));

            var sagaInstance = saga.Created.Select(p => p.CorrelationId == id).First();
            sagaInstance.Saga.VoiceChannelId = ((ulong)123456789).ToGuid();

            await harness.Bus.Publish(new UserJoinedVoiceChannelEvent { ChannelId = 123456789, UserId = 1111111111 });
            Thread.Sleep(500);
            Assert.Single(sagaInstance.Saga.UsersInVoiceChannel.Where(p => p == ((ulong)1111111111).ToGuid()));

            await harness.Bus.Publish(new UserLeftVoiceChannelEvent { ChannelId = 123456789, UserId = 1111111111 });
            Thread.Sleep(500);
            Assert.Empty(sagaInstance.Saga.UsersInVoiceChannel);
        }

        #endregion Public Methods
    }
}