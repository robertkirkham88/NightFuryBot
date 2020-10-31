namespace NFB.Domain.Bus.Events
{
    using NFB.Domain.Models;

    /// <summary>
    /// The vatsim pilot updated.
    /// </summary>
    public class VatsimPilotUpdatedEvent : BaseEvent
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the call sign.
        /// </summary>
        public string CallSign { get; set; }

        /// <summary>
        /// Gets or sets the destination airport.
        /// </summary>
        public string DestinationAirport { get; set; }

        /// <summary>
        /// Gets or sets the flight altitude.
        /// </summary>
        public int FlightAltitude { get; set; }

        /// <summary>
        /// Gets or sets the flight heading.
        /// </summary>
        public int FlightHeading { get; set; }

        /// <summary>
        /// Gets or sets the flight speed.
        /// </summary>
        public int FlightSpeed { get; set; }

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
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

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