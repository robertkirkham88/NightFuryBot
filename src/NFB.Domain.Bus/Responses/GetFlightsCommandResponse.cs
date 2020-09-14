namespace NFB.Domain.Bus.Responses
{
    using System.Collections.Generic;

    using NFB.Domain.Bus.DTOs;

    /// <summary>
    /// The get flights command response.
    /// </summary>
    public class GetFlightsCommandResponse
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the flights.
        /// </summary>
        public IEnumerable<FlightEntityDto> Flights { get; set; }

        #endregion Public Properties
    }
}