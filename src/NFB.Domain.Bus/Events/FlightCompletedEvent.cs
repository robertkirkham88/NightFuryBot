namespace NFB.Domain.Bus.Events
{
    using System;

    using NFB.Domain.Models;

    /// <summary>
    /// The flight completed event.
    /// </summary>
    public class FlightCompletedEvent : BaseEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        #endregion Public Properties
    }
}