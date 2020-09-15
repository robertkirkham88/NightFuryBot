namespace NFB.Service.Vatsim.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    /// <summary>
    /// The flight db context design time.
    /// </summary>
    public class VatsimDbContextDesignTime : IDesignTimeDbContextFactory<VatsimDbContext>
    {
        #region Public Methods

        /// <summary>
        /// Create a new design time context for migrations.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="VatsimDbContext"/>.
        /// </returns>
        public VatsimDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<VatsimDbContext>().UseNpgsql("Host=localhost;Database=VatsimService-Migrations-150920;Username=postgres").Options;
            return new VatsimDbContext(options);
        }

        #endregion Public Methods
    }
}