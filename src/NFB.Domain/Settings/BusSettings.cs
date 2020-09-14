namespace NFB.Domain.Settings
{
    /// <summary>
    /// The bus settings.
    /// </summary>
    public class BusSettings
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the bus host.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the bus pass.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the bus queue.
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// Gets or sets the bus user.
        /// </summary>
        public string Username { get; set; }

        #endregion Public Properties
    }
}