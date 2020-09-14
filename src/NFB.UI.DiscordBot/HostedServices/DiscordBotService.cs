namespace NFB.Ui.DiscordBot.HostedServices
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The discord bot service.
    /// </summary>
    public class DiscordBotService : IHostedService
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        /// <summary>
        /// The command service.
        /// </summary>
        private readonly CommandService commandService;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DiscordBotService> logger;

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// The token.
        /// </summary>
        private readonly string token;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordBotService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="commandService">
        /// The command service.
        /// </param>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public DiscordBotService(ILogger<DiscordBotService> logger, DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.logger = logger;
            this.client = client;
            this.commandService = commandService;
            this.serviceProvider = serviceProvider;
            this.token = configuration["BotToken"];
        }

        #endregion Public Constructors

        #region Private Properties

        /// <summary>
        /// The discord bot has successfully logged in.
        /// </summary>
        private Func<Task> OnLoggedIn =>
            () =>
                {
                    this.logger.LogInformation("Logged In");
                    return Task.CompletedTask;
                };

        /// <summary>
        /// The discord bot is ready.
        /// </summary>
        private Func<Task> OnReady =>
            async () =>
                {
                    await this.client.SetGameAsync("!help for commands");
                    this.logger.LogInformation("Ready");
                };

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Start the service.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Starting");

            this.client.LoggedIn += this.OnLoggedIn;
            this.client.Ready += this.OnReady;

            try
            {
                if (string.IsNullOrEmpty(this.token))
                    throw new Exception("Discord token is empty.");

                await this.client.LoginAsync(TokenType.Bot, this.token);
                await this.client.StartAsync();
                await this.commandService.AddModulesAsync(Assembly.GetExecutingAssembly(), this.serviceProvider);
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex.Message);
            }
        }

        /// <summary>
        /// Stop the bot.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger.LogInformation("Stopping");

            if (this.client.LoginState == LoginState.LoggedIn)
                await this.client.LogoutAsync();

            await this.client.StopAsync();

            this.client.LoggedIn -= this.OnLoggedIn;
            this.client.Ready -= this.OnReady;
        }

        #endregion Public Methods
    }
}