namespace NFB.Domain.Bus.Events
{
    using System;

    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Models;

    /// <summary>
    /// The flight created event.
    /// </summary>
    public class FlightCreatedEvent : BaseEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        public AirportEntityDto Destination { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public AirportEntityDto Origin { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        #endregion Public Properties
    }
}