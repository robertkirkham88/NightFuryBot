namespace NFB.Service.Flight.Tests.Consumers.Co
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit.Testing;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.Events;
    using NFB.Domain.Bus.Responses;
    using NFB.Service.Flight.Consumers.Commands;
    using NFB.Service.Flight.Tests.TestFactory;

    using Xunit;

    /// <summary>
    /// The create flight command consumer tests.
    /// </summary>
    public class CreateFlightCommandConsumerTests : ConsumerTestObject<CreateFlightCommandConsumer>
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFlightCommandConsumerTests"/> class.
        /// </summary>
        public CreateFlightCommandConsumerTests()
        {
            this.harness.Consumer(() => new CreateFlightCommandConsumer(this.database));
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
            mockConsumer.Verify(m => m.Publish(It.Is<FlightCreatedEvent>(p => p.Destination == command.Destination && p.Origin == command.Origin && p.StartTime == command.StartTime), It.IsAny<CancellationToken>()));
        }

        #endregion Public Methods
    }
}