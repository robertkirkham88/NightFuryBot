namespace NFB.Domain.Bus.Events
{
    /// <summary>
    /// The vatsim pilot updated.
    /// </summary>
    public class VatsimPilotUpdated
    {
        #region Public Properties

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
        /// Gets or sets the vatsim id.
        /// </summary>
        public string VatsimId { get; set; }

        #endregion Public Properties
    }
}