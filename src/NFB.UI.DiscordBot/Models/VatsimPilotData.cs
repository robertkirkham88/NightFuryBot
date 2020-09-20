namespace NFB.UI.DiscordBot.Models
{
    /// <summary>
    /// The vatsim pilot data.
    /// </summary>
    public class VatsimPilotData
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the assigned color.
        /// </summary>
        public uint AssignedColor { get; set; }

        /// <summary>
        /// Gets or sets the destination airport.
        /// </summary>
        public string DestinationAirport { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the origin airport.
        /// </summary>
        public string OriginAirport { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// Gets or sets the vatsim id.
        /// </summary>
        public string VatsimId { get; set; }

        #endregion Public Properties
    }
}