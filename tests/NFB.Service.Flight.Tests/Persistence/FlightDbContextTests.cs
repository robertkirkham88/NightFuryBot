namespace NFB.Service.Flight.Tests.Persistence
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using NFB.Service.Flight.Entities;
    using NFB.Service.Flight.Persistence;

    using Xunit;

    /// <summary>
    /// The flight database context tests.
    /// </summary>
    public class FlightDbContextTests
    {
        #region Private Fields

        /// <summary>
        /// The database.
        /// </summary>
        private readonly FlightDbContext database;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightDbContextTests"/> class.
        /// </summary>
        public FlightDbContextTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString());
            this.database = new FlightDbContext(optionsBuilder.Options);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Flight added successfully.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task FlightAdddedSuccessfully()
        {
            // Arrange
            var entity = new FlightEntity
            {
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddHours(3)
            };

            // Act
            var result = await this.database.Flights.AddAsync(entity);
            await this.database.SaveChangesAsync();

            // Assert
            Assert.Equal(entity, result.Entity);
            Assert.Equal(entity.Destination, result.Entity.Destination);
            Assert.Equal(entity.Origin, result.Entity.Origin);
            Assert.Equal(entity.StartTime, result.Entity.StartTime);
        }

        /// <summary>
        /// The flights database set not null.
        /// </summary>
        [Fact]
        public void FlightsNotNull()
        {
            // Assert
            Assert.NotNull(this.database.Flights);
        }

        #endregion Public Methods
    }
}