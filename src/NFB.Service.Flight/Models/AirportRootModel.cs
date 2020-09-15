namespace NFB.Service.Flight.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The airport root model.
    /// </summary>
    public class AirportRootModel
    {
        #region Public Fields

        /// <summary>
        /// The airports.
        /// </summary>
        [JsonProperty("airports")]
        public IEnumerable<AirportModel> Airports = new List<AirportModel>();

        #endregion Public Fields
    }
}