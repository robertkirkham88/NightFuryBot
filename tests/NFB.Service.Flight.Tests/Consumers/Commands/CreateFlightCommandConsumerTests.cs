namespace NFB.Service.Flight.Tests.Consumers.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit.Testing;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.Events;
    using NFB.Domain.Bus.Responses;
    using NFB.Service.Flight.Models;
    using NFB.Service.Flight.Persistence;
    using NFB.Service.Flight.Repository;
    using NFB.Service.Flight.Tests.TestFactory;

    using Xunit;

    /// <summary>
    /// The create flight command consumer tests.
    /// </summary>
    public class CreateFlightCommandConsumerTests
    {
        #region Private Fields

        /// <summary>
        /// The consumer.
        /// </summary>
        private readonly CreateFlightCommandConsumer consumer;

        /// <summary>
        /// The database.
        /// </summary>
        private readonly FlightDbContext database;

        /// <summary>
        /// The harness.
        /// </summary>
        private readonly InMemoryTestHarness harness;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFlightCommandConsumerTests"/> class.
        /// </summary>
        public CreateFlightCommandConsumerTests()
        {
            var airportRepository = new AirportRepository(new AirportRootModel
            {
                Airports = new List<AirportModel>
                 {
                     new AirportModel { ICAO = "EGCC", Name = "Manchester", Longitude = 50.4, Latitude = -50.4 },
                     new AirportModel { ICAO = "EGLL", Name = "London", Longitude = 70.4, Latitude = -70.4 },
                 }
            });

            // Constructors
            this.database = new FlightDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            this.consumer = new CreateFlightCommandConsumer(this.database, airportRepository);
            this.harness = new InMemoryTestHarness();
            this.harness.Consumer(() => this.consumer);
            this.harness.Start().GetAwaiter().GetResult();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The consumer consumes message.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ConsumerConsumesMessage()
        {
            // Arrange
            var command = new CreateFlightCommand
            {
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddHours(3)
            };

            // Act
            await this.harness.InputQueueSendEndpoint.Send(command);

            // Assert
            Assert.True(await this.harness.Consumed.Any<CreateFlightCommand>());
        }

        /// <summary>
        /// The execute added flight responds correctly.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ExecuteAddedFlightRespondsCorrectly()
        {
            // Arrange
            var command = new CreateFlightCommand
            {
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddHours(3)
            };
            var mockConsumer = new Mock<ConsumerContextTestObject<CreateFlightCommand>>(command);

            // Act
            await this.consumer.Consume(mockConsumer.Object);
            var result = await this.database.Flights.FirstAsync();

            // Assert
            mockConsumer.Verify(m => m.RespondAsync(It.Is<CreateFlightCommandSuccessResponse>(p => p.Id == result.Id)));
        }

        /// <summary>
        /// The consumer adds flight.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ExecuteAddsFlight()
        {
            // Arrange
            var command = new CreateFlightCommand
            {
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddHours(3)
            };
            var mockConsumer = new Mock<ConsumerContextTestObject<CreateFlightCommand>>(command);

            // Act
            await this.consumer.Consume(mockConsumer.Object);
            var result = await this.database.Flights.FirstAsync();

            // Assert
            Assert.Equal(command.Destination, result.Destination);
            Assert.Equal(command.Origin, result.Origin);
            Assert.Equal(command.StartTime, result.StartTime);
        }

        /// <summary>
        /// The execute fails validation.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Theory]
        [InlineData("EGCC", "EGL")]
        [InlineData("EGC", "EGLL")]
        public async Task ExecuteFailsValidation(string origin, string destination)
        {
            // Arrange
            var command = new CreateFlightCommand
            {
                Destination = destination,
                Origin = origin,
                StartTime = DateTime.UtcNow.AddHours(3)
            };
            var mockConsumer = new Mock<ConsumerContextTestObject<CreateFlightCommand>>(command);

            // Act
            await this.consumer.Consume(mockConsumer.Object);

            // Assert
            mockConsumer.Verify(m => m.RespondAsync(It.Is<CreateFlightCommandFailResponse>(p => !string.IsNullOrEmpty(p.Reason))));
        }

        /// <summary>
        /// The execute publishes event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ExecutePublishesEvent()
        {
            // Arrange
            var command = new CreateFlightCommand
            {
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddHours(3)
            };
            var mockConsumer = new Mock<ConsumerContextTestObject<CreateFlightCommand>>(command);

            // Act
            await this.consumer.Consume(mockConsumer.Object);

            // Assert
            mockConsumer.Verify(m => m.Publish(It.Is<FlightCreatedEvent>(p => p.Destination.ICAO == command.Destination && p.Origin.ICAO == command.Origin && p.StartTime == command.StartTime), It.IsAny<CancellationToken>()));
        }

        #endregion Public Methods
    }
}