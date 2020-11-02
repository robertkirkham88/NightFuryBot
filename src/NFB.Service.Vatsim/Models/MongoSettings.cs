namespace NFB.Service.Vatsim.Models
{
    using System.Security.Principal;

    /// <summary>
    /// The mongo settings.
    /// </summary>
    public class MongoSettings
    {
        #region Public Properties

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