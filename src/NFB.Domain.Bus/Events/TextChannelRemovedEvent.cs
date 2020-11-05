namespace NFB.Domain.Bus.Events
{
    /// <summary>
    /// The text channel removed event.
    /// </summary>
    public class TextChannelRemovedEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public ulong Id { get; set; }

        #endregion Public Properties
    }
}