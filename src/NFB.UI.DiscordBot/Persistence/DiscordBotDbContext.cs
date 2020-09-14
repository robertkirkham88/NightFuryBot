namespace NFB.UI.DiscordBot.Persistence
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The discord bot database context.
    /// </summary>
    public class DiscordBotDbContext : DbContext
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordBotDbContext"/> class.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        public DiscordBotDbContext(DbContextOptions options)
            : base(options)
        {
        }

        #endregion Public Constructors
    }
}