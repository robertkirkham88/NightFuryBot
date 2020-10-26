namespace NFB.UI.DiscordBot.Tests.StateMachines
{
    using System;
    using System.Threading.Tasks;

    using Discord;
    using Discord.WebSocket;

    using MassTransit;
    using MassTransit.Testing;

    using Microsoft.Extensions.DependencyInjection;

    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Activities;
    using NFB.UI.DiscordBot.StateMachines;
    using NFB.UI.DiscordBot.States;

    using Xunit;

    /// <summary>
    /// The flight state machine tests.
    /// </summary>
    public class FlightStateMachineTests : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        /// <summary>
        /// The harness.
        /// </summary>
        private readonly InMemoryTestHarness harness;

        /// <summary>
        /// The saga harness.
        /// </summary>
        private readonly IStateMachineSagaTestHarness<FlightState, FlightStateMachine> sagaHarness;

        /// <summary>
        /// The state machine.
        /// </summary>
        private readonly FlightStateMachine stateMachine;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightStateMachineTests"/> class.
        /// </summary>
        public FlightStateMachineTests()
        {
            // Get the token.
            var token = Environment.GetEnvironmentVariable("NFBTestToken", EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(token))
                throw new Exception("Token empty or not found");

            // Client
            this.client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Debug, DefaultRetryMode = RetryMode.AlwaysRetry });
            this.client.LoginAsync(TokenType.Bot, token).Wait();
            this.client.StartAsync().Wait();

            // Services
            var services = new ServiceCollection();
            services.AddMassTransitInMemoryTestHarness(
                cfg =>
                    {
                        cfg.AddSagaStateMachine<FlightStateMachine, FlightState>().InMemoryRepository();
                        cfg.AddSagaStateMachineTestHarness<FlightStateMachine, FlightState>();
                    });
            services.AddSingleton(this.client);
            services.AddScoped<CheckFlightCompletedActivity>();
            services.AddScoped<CreateDiscordChannelActivity>();
            services.AddScoped<CreateVoiceChannelActivity>();
            services.AddScoped<UpdateActiveFlightMessageActivity>();
            services.AddScoped<UpdateVatsimPilotDataActivity>();
            var provider = services.BuildServiceProvider(true);

            // Harness
            this.harness = provider.GetRequiredService<InMemoryTestHarness>();
            this.sagaHarness = provider.GetRequiredService<IStateMachineSagaTestHarness<FlightState, FlightStateMachine>>();
            this.stateMachine = new FlightStateMachine();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.client?.LogoutAsync().Wait();
            this.client?.Dispose();
        }

        /// <summary>
        /// Receive flight created event creates new saga.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ReceiveFlightCreatedEventCreatesNewSaga()
        {
            // Arrange
            await this.harness.Start();

            var flightCreatedEvent = new FlightCreatedEvent
            {
                Id = Guid.NewGuid(),
                Destination = new AirportEntityDto { ICAO = "EGCC" },
                Origin = new AirportEntityDto { ICAO = "EGLL" },
                StartTime = DateTime.UtcNow.AddHours(3)
            };

            // Act
            await this.harness.Bus.Publish(flightCreatedEvent);

            // Assert
            var sagaExists = await this.sagaHarness.Exists(flightCreatedEvent.Id, this.stateMachine.Created, TimeSpan.FromSeconds(5));
            Assert.Equal(flightCreatedEvent.Id, sagaExists.Value);

            await this.harness.Stop();
        }

        /// <summary>
        /// Receive out of order flight created event creates new saga.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ReceiveOutOfOrderFlightCreatedEventCreatesNewSaga()
        {
            // Arrange
            await this.harness.Start();
            var flightStartingEvent = new FlightStartingEvent
            {
                Id = Guid.NewGuid(),
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddHours(3)
            };
            var flightCreatedEvent = new FlightCreatedEvent
            {
                Id = flightStartingEvent.Id,
                Destination = new AirportEntityDto { ICAO = "EGCC" },
                Origin = new AirportEntityDto { ICAO = "EGLL" },
                StartTime = DateTime.UtcNow.AddHours(3)
            };

            // Act
            await this.harness.Bus.Publish(flightStartingEvent);
            await this.harness.Bus.Publish(flightCreatedEvent);

            // Assert
            var sagaExists = await this.sagaHarness.Exists(flightCreatedEvent.Id, this.stateMachine.Active, TimeSpan.FromSeconds(5));
            Assert.Equal(flightCreatedEvent.Id, sagaExists.Value);

            await this.harness.Stop();
        }

        #endregion Public Methods
    }
}