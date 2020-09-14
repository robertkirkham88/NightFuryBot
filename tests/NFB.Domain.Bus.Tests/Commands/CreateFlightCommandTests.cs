namespace NFB.Domain.Bus.Tests.Commands
{
    using System;

    using NFB.Domain.Bus.Commands;

    using Xunit;

    /// <summary>
    /// The create flight command tests.
    /// </summary>
    public class CreateFlightCommandTests
    {
        #region Public Methods

        /// <summary>
        /// The properties get set correctly.
        /// </summary>
        [Fact]
        public void PropertiesGetSetCorrectly()
        {
            // Arrange
            var destination = "EGCC";
            var origin = "EGLL";
            var startTime = DateTime.UtcNow.AddHours(1);

            // Act
            var command = new CreateFlightCommand
            {
                Destination = destination,
                Origin = origin,
                StartTime = startTime
            };

            // Assert
            Assert.Equal(destination, command.Destination);
            Assert.Equal(origin, command.Origin);
            Assert.Equal(startTime, command.StartTime);
        }

        #endregion Public Methods
    }
}