namespace NFB.Domain.Bus.DTOs
{
    /// <summary>
    /// The channel entity data transfer object.
    /// </summary>
    public class ChannelEntityDto
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the active flight channel.
        /// </summary>
        public ulong ActiveFlightMessageChannel { get; set; }

        /// <summary>
        /// Gets or sets the announcement channel.
        /// </summary>
        public ulong AnnouncementChannel { get; set; }

        /// <summary>
        /// Gets or sets the channel where the flight has been booked.
        /// </summary>
        public ulong BookChannel { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public ulong Category { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        #endregion Public Properties
    }
}