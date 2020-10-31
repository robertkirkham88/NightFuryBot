namespace NFB.Domain.Bus.Events
{
    using System;

    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Models;

    /// <summary>
    /// The flight submitted event.
    /// </summary>
    public class FlightSubmittedEvent : BaseEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the active message flight channel id.
        /// </summary>
        public ulong ActiveFlightMessageChannelId { get; set; }

        /// <summary>
        /// Gets or sets the channel data.
        /// </summary>
        public ChannelEntityDto ChannelData { get; set; }

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
        /// Gets or sets the original category id.
        /// </summary>
        public ulong RequestCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the request channel id.
        /// </summary>
        public ulong RequestChannelId { get; set; }

        /// <summary>
        /// Gets or sets the original request.
        /// </summary>
        public ulong RequestMessageId { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        #endregion Public Properties
    }
}