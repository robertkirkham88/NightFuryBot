namespace NFB.Domain.Bus.Tests.Events
{
    using System;

    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Bus.Events;

    using Xunit;

    /// <summary>
    /// The flight created event.
    /// </summary>
    public class FlightCreatedEventTests
    {
        #region Public Methods

        /// <summary>
        /// The properties get set correctly.
        /// </summary>
        [Fact]
        public void PropertiesGetSetCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var destination = "EGCC";
            var origin = "EGLL";
            var startTime = DateTime.UtcNow.AddHours(1);

            // Act
            var command = new FlightCreatedEvent()
            {
                Id = id,
                Destination = new AirportEntityDto { ICAO = destination },
                Origin = new AirportEntityDto { ICAO = origin },
                StartTime = startTime
            };

            // Assert
            Assert.Equal(id, command.Id);
            Assert.Equal(destination, command.Destination.ICAO);
            Assert.Equal(origin, command.Origin.ICAO);
            Assert.Equal(startTime, command.StartTime);
        }

        #endregion Public Methods
    }
}