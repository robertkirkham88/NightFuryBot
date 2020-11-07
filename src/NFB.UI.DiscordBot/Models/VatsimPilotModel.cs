namespace NFB.UI.DiscordBot.Models
{
    using System;

    /// <summary>
    /// The vatsim pilot data.
    /// </summary>
    public class VatsimPilotModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the flight altitude.
        /// </summary>
        public int Altitude { get; set; }

        /// <summary>
        /// Gets or sets the assigned color.
        /// </summary>
        public uint AssignedColor { get; set; }

        /// <summary>
        /// The atis message.
        /// </summary>
        public object AtisMessage { get; set; }

        /// <summary>
        /// Gets or sets the call sign.
        /// </summary>
        public string Callsign { get; set; }

        /// <summary>
        /// Gets or sets the vatsim id.
        /// </summary>
        public string Cid { get; set; }

        /// <summary>
        /// The clienttype.
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// The facilitytype.
        /// </summary>
        public int Facility { get; set; }

        /// <summary>
        /// The frequency.
        /// </summary>
        public object Frequency { get; set; }

        /// <summary>
        /// Gets or sets the flight speed.
        /// </summary>
        public int GroundSpeed { get; set; }

        /// <summary>
        /// Gets or sets the flight heading.
        /// </summary>
        public int Heading { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double Longitude { get; set; }

        public string PlannedActualDepartureTime { get; set; }

        /// <summary>
        /// The planned aircraft.
        /// </summary>
        public string PlannedAircraft { get; set; }

        /// <summary>
        /// The planned alternative airport.
        /// </summary>
        public string PlannedAlternativeAirport { get; set; }

        /// <summary>
        /// The planned altitude.
        /// </summary>
        public string PlannedAltitude { get; set; }

        /// <summary>
        /// Gets or sets the origin airport.
        /// </summary>
        public string PlannedDepartureAirport { get; set; }

        /// <summary>
        /// The planned departure airport latitude.
        /// </summary>
        public double PlannedDepartureAirportLatitude { get; set; }

        /// <summary>
        /// The planned departure airport longitude.
        /// </summary>
        public double PlannedDepartureAirportLongitude { get; set; }

        /// <summary>
        /// The planned departure time.
        /// </summary>
        public string PlannedDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets the destination airport.
        /// </summary>
        public string PlannedDestinationAirport { get; set; }

        /// <summary>
        /// The planned destination airport latitude.
        /// </summary>
        public double PlannedDestinationAirportLatitude { get; set; }

        /// <summary>
        /// The planned destination airport longitude.
        /// </summary>
        public double PlannedDestinationAirportLongitude { get; set; }

        /// <summary>
        /// The planned flight type.
        /// </summary>
        public string PlannedFlightType { get; set; }

        /// <summary>
        /// The planned hrsenroute.
        /// </summary>
        public string PlannedHrsenroute { get; set; }

        /// <summary>
        /// The planned hrs fuel.
        /// </summary>
        public string PlannedHrsFuel { get; set; }

        /// <summary>
        /// The planned minen route.
        /// </summary>
        public string PlannedMinenRoute { get; set; }

        /// <summary>
        /// The planned min fuel.
        /// </summary>
        public string PlannedMinFuel { get; set; }

        /// <summary>
        /// The planned remarks.
        /// </summary>
        public string PlannedRemarks { get; set; }

        /// <summary>
        /// The planned revision.
        /// </summary>
        public string PlannedRevision { get; set; }

        /// <summary>
        /// The planned route.
        /// </summary>
        public string PlannedRoute { get; set; }

        /// <summary>
        /// The planned tas cruise.
        /// </summary>
        public string PlannedTasCruise { get; set; }

        /// <summary>
        /// The prot revision.
        /// </summary>
        public int ProtRevision { get; set; }

        /// <summary>
        /// The QNH.
        /// </summary>
        public double QnhIHg { get; set; }

        /// <summary>
        /// The QNH Mb.
        /// </summary>
        public int QnhMb { get; set; }

        /// <summary>
        /// The rating.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// The real name.
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// The server.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The time last atis received.
        /// </summary>
        public DateTime TimeLastAtisReceived { get; set; }

        /// <summary>
        /// The time logon.
        /// </summary>
        public DateTime TimeLogon { get; set; }

        /// <summary>
        /// The transponder.
        /// </summary>
        public int Transponder { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// Gets or sets the vatsim id.
        /// </summary>
        public string VatsimId { get; set; }

        /// <summary>
        /// The visual range.
        /// </summary>
        public int VisualRange { get; set; }

        #endregion Public Properties
    }
}