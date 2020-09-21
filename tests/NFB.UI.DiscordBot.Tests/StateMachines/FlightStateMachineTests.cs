namespace NFB.UI.DiscordBot.Tests.StateMachines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac;
    using Autofac.Extensions.DependencyInjection;

    using Discord;
    using Discord.WebSocket;

    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.Testing;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Activities;
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
        #region Private Fields

        /// <summary>
        /// The harness.
        /// </summary>
        private readonly InMemoryTestHarness harness;

        private readonly ServiceProvider provider;

        /// <summary>
        /// The state machine.
        /// </summary>
        private readonly FlightStateMachine stateMachine;

        /// <summary>
        /// The saga.
        /// </summary>
        private StateMachineSagaTestHarness<FlightState, FlightStateMachine> saga;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightStateMachineTests"/> class.
        /// </summary>
        public FlightStateMachineTests()
        {
            // Mocks
            var mockMessage = new Mock<UserMessageTestObject>();
            var mockVoiceChannel = new Mock<VoiceChannelTestObject>();
            var mockTextChannel = new Mock<TextChannelTestObject>();
            var mockCategory = new Mock<CategoryChannelTestObject>();
            var mockGuild = new Mock<GuildTestObject>();
            var mockClient = new Mock<DiscordClientTestObject>();

            // Properties
            var mockCategoryObject = mockCategory.Object;
            var mockTextChannelObject = mockTextChannel.Object;
            var mockMessageObject = mockMessage.Object;
            var mockVoiceChannelObject = mockVoiceChannel.Object;
            var mockGuildObject = mockGuild.Object;
            var mockClientObject = mockClient.Object;

            mockMessageObject.Id = 789;
            mockVoiceChannelObject.Id = 456;
            mockTextChannelObject.Id = 123;
            mockTextChannelObject.Name = "flights";
            mockCategoryObject.Name = "flights";
            mockClientObject.Guilds = new List<IGuild> { mockGuildObject };

            // Setups
            mockTextChannel.Setup(p => p.SendMessageAsync(It.IsAny<Embed>())).ReturnsAsync(mockMessage.Object);
            mockGuild.Setup(p => p.CreateVoiceChannelAsync(It.IsAny<string>(), It.IsAny<Action<VoiceChannelProperties>>())).ReturnsAsync(mockVoiceChannel.Object);
            mockTextChannel.Setup(p => p.GetMessageAsync(It.IsAny<ulong>())).ReturnsAsync(mockMessage.Object);
            mockMessage.Setup(p => p.ModifyAsync(It.IsAny<Action<MessageProperties>>())).Returns(Task.CompletedTask);

            // Service provider
            var services = new ServiceCollection();
            services.AddMassTransit();

            services.AddSingleton<DiscordSocketClient>(mockClient.Object);
            services.AddScoped<CheckFlightCompletedActivity>();
            services.AddScoped<CreateDiscordChannelActivity>();
            services.AddScoped<CreateVoiceChannelActivity>();
            services.AddScoped<UpdateActiveFlightMessageActivity>();
            services.AddScoped<UpdateVatsimPilotDataActivity>();
            services.RegisterInMemorySagaRepository<FlightState>();
            services.RegisterSagaStateMachine<FlightStateMachine, FlightState>();

            this.provider = services.BuildServiceProvider();

            // Harness
            this.harness = new InMemoryTestHarness();
            this.stateMachine = new FlightStateMachine();
            this.harness.OnConfigureInMemoryReceiveEndpoint += cfg =>
            {
                cfg.StateMachineSaga(this.stateMachine, this.provider);
            };
        }

        #endregion Public Constructors

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
            await this.harness.Start();

            var id = Guid.NewGuid();

            await this.harness.Bus.Publish(
                new FlightCreatedEvent
                {
                    Id = id,
                    Destination = new AirportEntityDto { ICAO = "EGCC" },
                    Origin = new AirportEntityDto { ICAO = "EGLL" },
                    StartTime = DateTime.UtcNow.AddHours(3)
                });

            var repo = this.provider.GetService<ISagaRepository<FlightState>>() as InMemorySagaRepository<FlightState>;
            Assert.NotNull(repo);
            var sagaRepo = await repo.ShouldContainSagaInState(
                id,
                this.stateMachine,
                x => x.Created,
                TimeSpan.FromSeconds(5));

            Assert.NotNull(sagaRepo);

            await this.harness.Stop();
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
            await this.harness.Start();

            var id = Guid.NewGuid();

            await this.harness.Bus.Publish(
                new FlightCreatedEvent
                {
                    Id = id,
                    Destination = new AirportEntityDto { ICAO = "EGCC" },
                    Origin = new AirportEntityDto { ICAO = "EGLL" },
                    StartTime = DateTime.UtcNow.AddHours(3)
                });

            Assert.NotNull(await this.saga.Exists(id, this.stateMachine.Created, TimeSpan.FromSeconds(1)));

            var instance = this.saga.Created.Select(p => p.CorrelationId == id).FirstOrDefault();
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
            await this.harness.Start();

            var id = Guid.NewGuid();

            await this.harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = new AirportEntityDto { ICAO = "EGCC" },
                Origin = new AirportEntityDto { ICAO = "EGLL" },
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            Assert.NotNull(await this.saga.Exists(id, this.stateMachine.Created, TimeSpan.FromSeconds(1)));
            Assert.Null(await this.saga.Exists(id, this.stateMachine.Active, TimeSpan.FromSeconds(1)));

            await this.harness.Bus.Publish(new FlightStartingEvent
            {
                Id = id,
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            Assert.NotNull(await this.saga.Exists(id, this.stateMachine.Active, TimeSpan.FromSeconds(1)));

            await this.harness.Stop();
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
            await this.harness.Start();

            var id = Guid.NewGuid();

            await this.harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = new AirportEntityDto { ICAO = "EGCC" },
                Origin = new AirportEntityDto { ICAO = "EGLL" },
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            Assert.NotNull(await this.saga.Exists(id, this.stateMachine.Created, TimeSpan.FromSeconds(1)));
            Assert.Null(await this.saga.Exists(id, this.stateMachine.Active, TimeSpan.FromSeconds(1)));

            await this.harness.Bus.Publish(new FlightStartingEvent
            {
                Id = id,
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            await this.saga.Exists(id, this.stateMachine.Active, TimeSpan.FromSeconds(1));

            var sagaInstance = this.saga.Created.Select(p => p.CorrelationId == id).First();
            sagaInstance.Saga.VoiceChannelId = ((ulong)123456789).ToGuid();

            await this.harness.Bus.Publish(new UserJoinedVoiceChannelEvent { ChannelId = 123456789, UserId = 1111111111 });
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
            await this.harness.Start();

            var id = Guid.NewGuid();

            await this.harness.Bus.Publish(new FlightCreatedEvent
            {
                Id = id,
                Destination = new AirportEntityDto { ICAO = "EGCC" },
                Origin = new AirportEntityDto { ICAO = "EGLL" },
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            Assert.NotNull(await this.saga.Exists(id, this.stateMachine.Created, TimeSpan.FromSeconds(1)));
            Assert.Null(await this.saga.Exists(id, this.stateMachine.Active, TimeSpan.FromSeconds(1)));

            await this.harness.Bus.Publish(new FlightStartingEvent
            {
                Id = id,
                StartTime = DateTime.UtcNow.AddHours(3)
            });

            await this.saga.Exists(id, this.stateMachine.Active, TimeSpan.FromSeconds(1));

            var sagaInstance = this.saga.Created.Select(p => p.CorrelationId == id).First();
            sagaInstance.Saga.VoiceChannelId = ((ulong)123456789).ToGuid();

            await this.harness.Bus.Publish(new UserJoinedVoiceChannelEvent { ChannelId = 123456789, UserId = 1111111111 });
            Thread.Sleep(500);
            Assert.Single(sagaInstance.Saga.UsersInVoiceChannel.Where(p => p == ((ulong)1111111111).ToGuid()));

            await this.harness.Bus.Publish(new UserLeftVoiceChannelEvent { ChannelId = 123456789, UserId = 1111111111 });
            Thread.Sleep(500);
            Assert.Empty(sagaInstance.Saga.UsersInVoiceChannel);
        }

        #endregion Public Methods
    }
}