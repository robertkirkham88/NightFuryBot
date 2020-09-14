namespace NFB.Service.Flight.Tests.States
{
    using System;

    using NFB.Service.Flight.States;

    using Xunit;

    /// <summary>
    /// The flight state tests.
    /// </summary>
    public class FlightStateTests
    {
        #region Public Methods

        /// <summary>
        /// The properties get set correctly.
        /// </summary>
        [Fact]
        public void PropertiesGetSetCorrectly()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            var currentState = "Created";
            var flightStartedScheduledToken = Guid.NewGuid();
            var flightStartingScheduledToken = Guid.NewGuid();

            // Act
            var state = new FlightState
            {
                CorrelationId = correlationId,
                CurrentState = currentState,
                FlightStartingScheduleToken = flightStartingScheduledToken,
                FlightStartedScheduledToken = flightStartedScheduledToken
            };

            // Assert
            Assert.Equal(correlationId, state.CorrelationId);
            Assert.Equal(currentState, state.CurrentState);
            Assert.Equal(flightStartingScheduledToken, state.FlightStartingScheduleToken);
            Assert.Equal(flightStartedScheduledToken, state.FlightStartedScheduledToken);
        }

        #endregion Public Methods
    }
}