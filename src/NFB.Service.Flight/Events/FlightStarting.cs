namespace NFB.Service.Flight.Events
{
    using System;

    /// <summary>
    /// The flight starting.
    /// </summary>
    public class FlightStarting
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        #endregion Public Properties
    }
}