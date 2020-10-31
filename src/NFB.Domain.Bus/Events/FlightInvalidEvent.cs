namespace NFB.Domain.Bus.Events
{
    using System;

    /// <summary>
    /// The flight invalid event.
    /// </summary>
    public class FlightInvalidEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the validation error.
        /// </summary>
        public string ValidationError { get; set; }

        #endregion Public Properties
    }
}