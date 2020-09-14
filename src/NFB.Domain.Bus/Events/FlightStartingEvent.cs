namespace NFB.Domain.Bus.Events
{
    using System;

    using NFB.Domain.Models;

    /// <summary>
    /// The flight starting event.
    /// </summary>
    public class FlightStartingEvent : BaseEvent
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