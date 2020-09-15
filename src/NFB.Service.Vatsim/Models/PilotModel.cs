namespace NFB.Service.Vatsim.Models
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// The pilot model.
    /// </summary>
    public class PilotModel
    {
        #region Public Fields

        /// <summary>
        /// The altitude.
        /// </summary>
        [JsonProperty("altitude")]
        public int Altitude;

        /// <summary>
        /// The atis message.
        /// </summary>
        [JsonProperty("atis_message")]
        public object AtisMessage;

        /// <summary>
        /// The callsign.
        /// </summary>
        [JsonProperty("callsign")]
        public string Callsign;

        /// <summary>
        /// The cid.
        /// </summary>
        [JsonProperty("cid")]
        public string Cid;

        /// <summary>
        /// The clienttype.
        /// </summary>
        [JsonProperty("clienttype")]
        public string Client;

        /// <summary>
        /// The facilitytype.
        /// </summary>
        [JsonProperty("facilitytype")]
        public int Facility;

        /// <summary>
        /// The frequency.
        /// </summary>
        [JsonProperty("frequency")]
        public object Frequency;

        /// <summary>
        /// The ground speed.
        /// </summary>
        [JsonProperty("groundspeed")]
        public int GroundSpeed;

        /// <summary>
        /// The heading.
        /// </summary>
        [JsonProperty("heading")]
        public int Heading;

        /// <summary>
        /// The latitude.
        /// </summary>
        [JsonProperty("latitude")]
        public double Latitude;

        /// <summary>
        /// The longitude.
        /// </summary>
        [JsonProperty("longitude")]
        public double Longitude;

        [JsonProperty("planned_actdeptime")]
        public string PlannedActualDepartureTime;

        /// <summary>
        /// The planned aircraft.
        /// </summary>
        [JsonProperty("planned_aircraft")]
        public string PlannedAircraft;

        /// <summary>
        /// The planned alternative airport.
        /// </summary>
        [JsonProperty("planned_altairport")]
        public string PlannedAlternativeAirport;

        /// <summary>
        /// The planned altitude.
        /// </summary>
        [JsonProperty("planned_altitude")]
        public string PlannedAltitude;

        /// <summary>
        /// The planned departure airport.
        /// </summary>
        [JsonProperty("planned_depairport")]
        public string PlannedDepartureAirport;

        /// <summary>
        /// The planned departure airport latitude.
        /// </summary>
        [JsonProperty("planned_depairport_lat")]
        public double PlannedDepartureAirportLatitude;

        /// <summary>
        /// The planned departure airport longitude.
        /// </summary>
        [JsonProperty("planned_depairport_lon")]
        public double PlannedDepartureAirportLongitude;

        /// <summary>
        /// The planned departure time.
        /// </summary>
        [JsonProperty("planned_deptime")]
        public string PlannedDepartureTime;

        /// <summary>
        /// The planned destination airport.
        /// </summary>
        [JsonProperty("planned_destairport")]
        public string PlannedDestinationAirport;

        /// <summary>
        /// The planned destination airport latitude.
        /// </summary>
        [JsonProperty("planned_destairport_lat")]
        public double PlannedDestinationAirportLatitude;

        /// <summary>
        /// The planned destination airport longitude.
        /// </summary>
        [JsonProperty("planned_destairport_lon")]
        public double PlannedDestinationAirportLongitude;

        /// <summary>
        /// The planned flight type.
        /// </summary>
        [JsonProperty("planned_flighttype")]
        public string PlannedFlightType;

        /// <summary>
        /// The planned hrsenroute.
        /// </summary>
        [JsonProperty("planned_hrsenroute")]
        public string PlannedHrsenroute;

        /// <summary>
        /// The planned hrs fuel.
        /// </summary>
        [JsonProperty("planned_hrsfuel")]
        public string PlannedHrsFuel;

        /// <summary>
        /// The planned minen route.
        /// </summary>
        [JsonProperty("planned_minenroute")]
        public string PlannedMinenRoute;

        /// <summary>
        /// The planned min fuel.
        /// </summary>
        [JsonProperty("planned_minfuel")]
        public string PlannedMinFuel;

        /// <summary>
        /// The planned remarks.
        /// </summary>
        [JsonProperty("planned_remarks")]
        public string PlannedRemarks;

        /// <summary>
        /// The planned revision.
        /// </summary>
        [JsonProperty("planned_revision")]
        public string PlannedRevision;

        /// <summary>
        /// The planned route.
        /// </summary>
        [JsonProperty("planned_route")]
        public string PlannedRoute;

        /// <summary>
        /// The planned tas cruise.
        /// </summary>
        [JsonProperty("planned_tascruise")]
        public string PlannedTasCruise;

        /// <summary>
        /// The prot revision.
        /// </summary>
        [JsonProperty("protrevision")]
        public int ProtRevision;

        /// <summary>
        /// The QNH.
        /// </summary>
        [JsonProperty("qnh_i_hg")]
        public double QnhIHg;

        /// <summary>
        /// The QNH Mb.
        /// </summary>
        [JsonProperty("qnh_mb")]
        public int QnhMb;

        /// <summary>
        /// The rating.
        /// </summary>
        [JsonProperty("rating")]
        public int Rating;

        /// <summary>
        /// The real name.
        /// </summary>
        [JsonProperty("realname")]
        public string RealName;

        /// <summary>
        /// The server.
        /// </summary>
        [JsonProperty("server")]
        public string Server;

        /// <summary>
        /// The time last atis received.
        /// </summary>
        [JsonProperty("time_last_atis_received")]
        public DateTime TimeLastAtisReceived;

        /// <summary>
        /// The time logon.
        /// </summary>
        [JsonProperty("time_logon")]
        public DateTime TimeLogon;

        /// <summary>
        /// The transponder.
        /// </summary>
        [JsonProperty("transponder")]
        public int Transponder;

        /// <summary>
        /// The visual range.
        /// </summary>
        [JsonProperty("visualrange")]
        public int VisualRange;

        #endregion Public Fields
    }
}