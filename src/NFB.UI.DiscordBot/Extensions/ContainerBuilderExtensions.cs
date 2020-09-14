namespace NFB.Ui.DiscordBot.Extensions
{
    using Autofac;

    using Discord.Commands;
    using Discord.WebSocket;

    /// <summary>
    /// The container builder extensions.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        #region Public Methods

        /// <summary>
        /// Add a command service.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public static void AddCommandService(this ContainerBuilder builder)
        {
            var config = new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false
            };

            var client = new CommandService(config);

            builder.RegisterInstance(client).SingleInstance();
        }

        /// <summary>
        /// Add a discord service to the container
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public static void AddDiscordService(this ContainerBuilder builder)
        {
            var config = new DiscordSocketConfig
            {
                MessageCacheSize = 50
            };

            var client = new DiscordSocketClient(config);

            builder.RegisterInstance(client).SingleInstance();
        }

        #endregion Public Methods
    }
}