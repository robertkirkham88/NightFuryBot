namespace NFB.Domain.Bus.Events
{
    /// <summary>
    /// The user left voice channel event.
    /// </summary>
    public class UserLeftVoiceChannelEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the channel id.
        /// </summary>
        public ulong ChannelId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public ulong UserId { get; set; }

        #endregion Public Properties
    }
}