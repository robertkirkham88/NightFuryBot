namespace NFB.Service.Vatsim.Persistence
{
    using Microsoft.EntityFrameworkCore;

    using NFB.Service.Vatsim.Entities;

    /// <summary>
    /// The flight database context.
    /// </summary>
    public class VatsimDbContext : DbContext
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VatsimDbContext"/> class.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        public VatsimDbContext(DbContextOptions options)
            : base(options)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the pilots.
        /// </summary>
        public DbSet<PilotEntity> Pilots { get; set; }

        #endregion Public Properties
    }
}