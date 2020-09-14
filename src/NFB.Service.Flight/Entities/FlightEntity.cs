namespace NFB.Service.Flight.Entities
{
    using System;

    using NFB.Domain.Models;

    /// <summary>
    /// The flight entity.
    /// </summary>
    public class FlightEntity : BaseEntity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Gets or sets the UTC start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        #endregion Public Properties
    }
}