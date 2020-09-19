namespace NFB.Domain.Bus.DTOs
{
    /// <summary>
    /// The airport entity data transfer object.
    /// </summary>
    public class AirportEntityDto
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the ICAO.
        /// </summary>
        public string ICAO { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        #endregion Public Properties
    }
}