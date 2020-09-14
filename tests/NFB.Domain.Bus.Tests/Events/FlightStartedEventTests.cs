namespace NFB.Domain.Bus.Tests.Events
{
    using System;

    using NFB.Domain.Bus.Events;

    using Xunit;

    /// <summary>
    /// The flight started event tests.
    /// </summary>
    public class FlightStartedEventTests
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

            // Act
            var command = new FlightStartedEvent
            {
                Id = id
            };

            // Assert
            Assert.Equal(id, command.Id);
        }

        #endregion Public Methods
    }
}