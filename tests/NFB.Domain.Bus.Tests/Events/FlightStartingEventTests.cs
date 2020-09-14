namespace NFB.Domain.Bus.Tests.Events
{
    using System;

    using NFB.Domain.Bus.Events;

    using Xunit;

    /// <summary>
    /// The flight starting event.
    /// </summary>
    public class FlightStartingEventTests
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
            var origin = "EGCC";
            var destination = "EGLL";
            var startTime = DateTime.UtcNow.AddHours(3);

            // Act
            var command = new FlightStartingEvent
            {
                Id = id,
                Origin = origin,
                Destination = destination,
                StartTime = startTime
            };

            // Assert
            Assert.Equal(id, command.Id);
            Assert.Equal(origin, command.Origin);
            Assert.Equal(destination, command.Destination);
            Assert.Equal(startTime, command.StartTime);
        }

        #endregion Public Methods
    }
}