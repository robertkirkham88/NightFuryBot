namespace NFB.Service.Flight.Persistence
{
    using Microsoft.EntityFrameworkCore;

    using NFB.Service.Flight.Entities;

    /// <summary>
    /// The flight database context.
    /// </summary>
    public class FlightDbContext : DbContext
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightDbContext"/> class.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        public FlightDbContext(DbContextOptions options)
            : base(options)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the flights.
        /// </summary>
        public DbSet<FlightEntity> Flights { get; set; }

        #endregion Public Properties
    }
}