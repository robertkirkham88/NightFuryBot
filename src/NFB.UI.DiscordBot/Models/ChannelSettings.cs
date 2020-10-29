namespace NFB.UI.DiscordBot.Models
{
    /// <summary>
    /// The channel settings.
    /// </summary>
    public class ChannelSettings
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the collection name.
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName { get; set; }

        #endregion Public Properties
    }
}