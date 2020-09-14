namespace NFB.Domain.Models
{
    using System;

    /// <summary>
    /// The base event.
    /// </summary>
    public class BaseEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the date occurred.
        /// </summary>
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;

        #endregion Public Properties
    }
}