namespace NFB.Service.Flight.Repository
{
    using System.Collections.Generic;
    using System.Linq;

    using NFB.Service.Flight.Models;

    /// <summary>
    /// The airport repository.
    /// </summary>
    public class AirportRepository
    {
        #region Private Fields

        /// <summary>
        /// The airports.
        /// </summary>
        private readonly IEnumerable<AirportModel> airports;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AirportRepository"/> class.
        /// </summary>
        /// <param name="airports">
        /// The airports.
        /// </param>
        public AirportRepository(AirportRootModel airports)
        {
            this.airports = airports.Airports;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Does the ICAO exist for a particular airport.
        /// </summary>
        /// <param name="icao">
        /// The destination ICAO.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Exists(string icao)
        {
            return this.airports.Any(p => p.ICAO == icao.ToUpper());
        }

        /// <summary>
        /// Get an airport by ICAO.
        /// </summary>
        /// <param name="icao">
        /// The icao.
        /// </param>
        /// <returns>
        /// The <see cref="AirportModel"/>.
        /// </returns>
        public AirportModel GetAirport(string icao)
        {
            return this.airports.FirstOrDefault(p => p.ICAO == icao.ToUpper());
        }

        #endregion Public Methods
    }
}