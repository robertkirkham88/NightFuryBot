namespace NFB.Domain.Bus.Events
{
    /// <summary>
    /// The discord message expired.
    /// </summary>
    public class DiscordMessageExpiredEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the channel id.
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        public ulong MessageId { get; set; }

        #endregion Public Properties
    }
}