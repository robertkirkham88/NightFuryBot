namespace NFB.Domain.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The base entity.
    /// </summary>
    public class BaseEntity
    {
        #region Public Fields

        /// <summary>
        /// The events.
        /// </summary>
        public IList<BaseEvent> Events = new List<BaseEvent>();

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        #endregion Public Properties
    }
}