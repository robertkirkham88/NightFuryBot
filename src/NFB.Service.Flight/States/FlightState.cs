namespace NFB.Service.Flight.States
{
    using System;

    using Automatonymous;

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
        /// Gets or sets the flight started scheduled token.
        /// </summary>
        public Guid? FlightStartedScheduledToken { get; set; }

        /// <summary>
        /// Gets or sets the flight starting schedule token.
        /// </summary>
        public Guid? FlightStartingScheduleToken { get; set; }

        #endregion Public Properties
    }
}