namespace NFB.Service.Flight.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    /// <summary>
    /// The flight db context design time.
    /// </summary>
    public class FlightDbContextDesignTime : IDesignTimeDbContextFactory<FlightDbContext>
    {
        #region Public Methods

        /// <summary>
        /// Create a new design time context for migrations.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="FlightDbContext"/>.
        /// </returns>
        public FlightDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<FlightDbContext>().UseNpgsql("Host=localhost;Database=FlightService-Migrations-130920;Username=superuser;Password=z1Tf3U9io5ufol").Options;
            return new FlightDbContext(options);
        }

        #endregion Public Methods
    }
}