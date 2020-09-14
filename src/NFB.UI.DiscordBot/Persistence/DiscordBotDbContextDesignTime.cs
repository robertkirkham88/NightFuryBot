namespace NFB.UI.DiscordBot.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    /// <summary>
    /// The discord bot db context design time.
    /// </summary>
    public class DiscordBotDbContextDesignTime : IDesignTimeDbContextFactory<DiscordBotDbContext>
    {
        #region Public Methods

        /// <summary>
        /// Create a new design time context for migrations.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="DiscordBotDbContext"/>.
        /// </returns>
        public DiscordBotDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<DiscordBotDbContext>().UseNpgsql("Host=localhost;Database=DiscordBot-Migrations-140920;Username=superuser;Password=z1Tf3U9io5ufol").Options;
            return new DiscordBotDbContext(options);
        }

        #endregion Public Methods
    }
}