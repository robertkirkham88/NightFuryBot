namespace NFB.Domain.Bus.Events
{
    /// <summary>
    /// The voice channel removed event.
    /// </summary>
    public class VoiceChannelRemovedEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public ulong Id { get; set; }

        #endregion Public Properties
    }
}