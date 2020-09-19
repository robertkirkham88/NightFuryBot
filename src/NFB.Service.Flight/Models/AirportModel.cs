namespace NFB.Service.Flight.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// The airport model.
    /// </summary>
    public class AirportModel
    {
        #region Public Fields

        /// <summary>
        /// The altitude.
        /// </summary>
        [JsonProperty("Altitude")]
        public int Altitude;

        /// <summary>
        /// The city.
        /// </summary>
        [JsonProperty("City")]
        public string City;

        /// <summary>
        /// The country.
        /// </summary>
        [JsonProperty("Country")]
        public string Country;

        /// <summary>
        /// The dst.
        /// </summary>
        [JsonProperty("DST")]
        public string DST;

        /// <summary>
        /// The iata.
        /// </summary>
        [JsonProperty("IATA")]
        public string IATA;

        /// <summary>
        /// The icao.
        /// </summary>
        [JsonProperty("ICAO")]
        public string ICAO;

        /// <summary>
        /// The id.
        /// </summary>
        [JsonProperty("ID")]
        public int ID;

        /// <summary>
        /// The latitude.
        /// </summary>
        [JsonProperty("Latitude")]
        public double Latitude;

        /// <summary>
        /// The longitude.
        /// </summary>
        [JsonProperty("Longitude")]
        public double Longitude;

        /// <summary>
        /// The name.
        /// </summary>
        [JsonProperty("Name")]
        public string Name;

        /// <summary>
        /// The source.
        /// </summary>
        [JsonProperty("Source")]
        public string Source;

        /// <summary>
        /// The timezone.
        /// </summary>
        [JsonProperty("Timezone")]
        public string Timezone;

        /// <summary>
        /// The type.
        /// </summary>
        [JsonProperty("Type")]
        public string Type;

        /// <summary>
        /// The database time zone.
        /// </summary>
        [JsonProperty("TzDatabaseTimeZon")]
        public string TzDatabaseTimeZone;

        #endregion Public Fields
    }
}