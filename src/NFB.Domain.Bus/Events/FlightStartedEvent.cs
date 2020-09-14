namespace NFB.Domain.Bus.Events
{
    using System;

    using NFB.Domain.Models;

    /// <summary>
    /// The flight started event.
    /// </summary>
    public class FlightStartedEvent : BaseEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        #endregion Public Properties
    }
}