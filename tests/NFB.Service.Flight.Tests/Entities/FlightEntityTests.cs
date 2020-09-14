namespace NFB.Service.Flight.Tests.Entities
{
    using System;

    using NFB.Service.Flight.Entities;

    using Xunit;

    /// <summary>
    /// The flight entity tests.
    /// </summary>
    public class FlightEntityTests
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
            var startTime = DateTime.UtcNow.AddHours(3);

            // Act
            var entity = new FlightEntity
            {
                Id = id,
                Destination = destination,
                Origin = origin,
                StartTime = startTime
            };

            // Assert
            Assert.Equal(id, entity.Id);
            Assert.Equal(destination, entity.Destination);
            Assert.Equal(origin, entity.Origin);
            Assert.Equal(startTime, entity.StartTime);
        }

        #endregion Public Methods
    }
}