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
        /// The carriers.
        /// </summary>
        [JsonProperty("carriers")]
        public string Carriers;

        /// <summary>
        /// The city.
        /// </summary>
        [JsonProperty("city")]
        public string City;

        /// <summary>
        /// The code.
        /// </summary>
        [JsonProperty("code")]
        public string Code;

        /// <summary>
        /// The country.
        /// </summary>
        [JsonProperty("country")]
        public string Country;

        /// <summary>
        /// The direct flights.
        /// </summary>
        [JsonProperty("direct_flights")]
        public string DirectFlights;

        /// <summary>
        /// The elevation.
        /// </summary>
        [JsonProperty("elev")]
        public string Elevation;

        /// <summary>
        /// The email.
        /// </summary>
        [JsonProperty("email")]
        public string Email;

        /// <summary>
        /// The icao.
        /// </summary>
        [JsonProperty("icao")]
        public string ICAO;

        /// <summary>
        /// The latitude.
        /// </summary>
        [JsonProperty("lat")]
        public string Latitude;

        /// <summary>
        /// The longitude.
        /// </summary>
        [JsonProperty("lon")]
        public string Longitude;

        /// <summary>
        /// The name.
        /// </summary>
        [JsonProperty("name")]
        public string Name;

        /// <summary>
        /// The phone.
        /// </summary>
        [JsonProperty("phone")]
        public string Phone;

        /// <summary>
        /// The runway length.
        /// </summary>
        [JsonProperty("runway_length")]
        public string RunwayLength;

        /// <summary>
        /// The state.
        /// </summary>
        [JsonProperty("state")]
        public string State;

        /// <summary>
        /// The timezone.
        /// </summary>
        [JsonProperty("tz")]
        public string Timezone;

        /// <summary>
        /// The type.
        /// </summary>
        [JsonProperty("type")]
        public string Type;

        /// <summary>
        /// The url.
        /// </summary>
        [JsonProperty("url")]
        public string URL;

        /// <summary>
        /// The woe id.
        /// </summary>
        [JsonProperty("woeid")]
        public string WOEId;

        #endregion Public Fields
    }
}