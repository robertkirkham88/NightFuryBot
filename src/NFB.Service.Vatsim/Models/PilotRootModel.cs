namespace NFB.Service.Vatsim.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The pilot root model.
    /// </summary>
    public class PilotRootModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the pilots.
        /// </summary>
        [JsonProperty("clients")]
        public IEnumerable<PilotModel> Pilots { get; set; } = new List<PilotModel>();

        #endregion Public Properties
    }
}