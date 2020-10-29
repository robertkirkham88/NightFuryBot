namespace NFB.Service.Flight.States
{
    using System;

    using Automatonymous;

    using NFB.Service.Flight.Models;

    /// <summary>
    /// The flight state.
    /// </summary>
    public class FlightState : SagaStateMachineInstance
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the correlation id.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the current state.
        /// </summary>
        public string CurrentState { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        public AirportModel Destination { get; set; }

        /// <summary>
        /// Gets or sets the flight started scheduled token.
        /// </summary>
        public Guid? FlightStartedScheduledToken { get; set; }

        /// <summary>
        /// Gets or sets the flight starting schedule token.
        /// </summary>
        public Guid? FlightStartingScheduleToken { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public AirportModel Origin { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        #endregion Public Properties
    }
}