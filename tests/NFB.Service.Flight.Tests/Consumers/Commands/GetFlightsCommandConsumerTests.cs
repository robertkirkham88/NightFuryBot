namespace NFB.Service.Flight.Tests.Consumers.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MassTransit.Testing;

    using Moq;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Bus.Responses;
    using NFB.Service.Flight.Consumers.Commands;
    using NFB.Service.Flight.Entities;
    using NFB.Service.Flight.Tests.TestFactory;

    using Xunit;

    /// <summary>
    /// The get flights command consumer tests.
    /// </summary>
    public class GetFlightsCommandConsumerTests : ConsumerTestObject<GetFlightsCommandConsumer>
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFlightsCommandConsumerTests"/> class.
        /// </summary>
        public GetFlightsCommandConsumerTests()
        {
            this.harness.Consumer(() => new GetFlightsCommandConsumer(this.database));
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
            var command = new GetFlightsCommand();

            // Act
            await this.harness.InputQueueSendEndpoint.Send(command);

            // Assert
            Assert.True(await this.harness.Consumed.Any<GetFlightsCommand>());
        }

        /// <summary>
        /// The execute gets list of flights.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ExecuteGetsListOfFlights()
        {
            // Arrange
            await this.database.Flights.AddRangeAsync(new List<FlightEntity>
                                                          {
                                                              new FlightEntity
                                                                  {
                                                                      Destination = "EGCC", Origin = "EGLL", StartTime = DateTime.UtcNow.AddHours(1)
                                                                  },
                                                              new FlightEntity
                                                                  {
                                                                      Destination = "EGCC", Origin = "EGLL", StartTime = DateTime.UtcNow.AddHours(2)
                                                                  },
                                                              new FlightEntity
                                                                  {
                                                                      Destination = "EGCC", Origin = "EGLL", StartTime = DateTime.UtcNow.AddHours(3)
                                                                  },
                                                          });
            await this.database.SaveChangesAsync();
            var flights = this.database.Flights.Select(p => new FlightEntityDto
            {
                Destination = p.Destination,
                Origin = p.Origin,
                Id = p.Id,
                StartTime = p.StartTime
            });

            var command = new GetFlightsCommand();
            var mockConsumer = new Mock<ConsumerContextTestObject<GetFlightsCommand>>(command);

            // Act
            await this.consumer.Consume(mockConsumer.Object);

            // Assert
            Assert.All(
                flights,
                flight =>
                    {
                        mockConsumer.Verify(m => m.RespondAsync(
                            It.Is<GetFlightsCommandResponse>(
                                p =>
                                    p.Flights.Any(l => l.Id == flight.Id && l.Destination == flight.Destination && l.Origin == flight.Origin && l.StartTime == flight.StartTime))));
                    });
        }

        #endregion Public Methods
    }
}