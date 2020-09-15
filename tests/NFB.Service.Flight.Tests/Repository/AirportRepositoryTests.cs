namespace NFB.Service.Flight.Tests.Repository
{
    using System.Collections.Generic;

    using NFB.Service.Flight.Models;
    using NFB.Service.Flight.Repository;

    using Xunit;

    /// <summary>
    /// The airport repository tests.
    /// </summary>
    public class AirportRepositoryTests
    {
        #region Private Fields

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly AirportRepository repository;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AirportRepositoryTests"/> class.
        /// </summary>
        public AirportRepositoryTests()
        {
            var airports = new List<AirportModel>
            {
               new AirportModel { ICAO = "EGCC" },
               new AirportModel { ICAO = "EGLL" },
               new AirportModel { ICAO = "EGKK" }
            };
            this.repository = new AirportRepository(new AirportRootModel { Airports = airports });
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The airport exists returns correct value.
        /// </summary>
        /// <param name="icao">
        /// The icao.
        /// </param>
        /// <param name="exists">
        /// The exists.
        /// </param>
        [Theory]
        [InlineData("EGCC", true)]
        [InlineData("EGLL", true)]
        [InlineData("EGKK", true)]
        [InlineData("EGPH", false)]
        public void AirportExistsReturnsCorrectValue(string icao, bool exists)
        {
            // Act
            var result = this.repository.Exists(icao);

            // Assert
            Assert.Equal(exists, result);
        }

        /// <summary>
        /// Get airport returns the an airport.
        /// </summary>
        /// <param name="icao">
        /// The icao.
        /// </param>
        [Theory]
        [InlineData("EGCC")]
        [InlineData("EGLL")]
        [InlineData("EGKK")]
        public void GetAirportReturnsAirport(string icao)
        {
            // Act
            var result = this.repository.GetAirport(icao);

            // Assert
            Assert.Equal(icao, result.ICAO);
        }

        /// <summary>
        /// Get airport returns null when airport not found.
        /// </summary>
        /// <param name="icao">
        /// The icao.
        /// </param>
        [Theory]
        [InlineData("EGPP")]
        [InlineData("EGPH")]
        public void GetAirportReturnsNullWhenAirportNotFound(string icao)
        {
            // Act
            var result = this.repository.GetAirport(icao);

            // Assert
            Assert.Null(result);
        }

        #endregion Public Methods
    }
}