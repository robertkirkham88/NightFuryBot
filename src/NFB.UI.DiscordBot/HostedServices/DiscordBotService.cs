namespace NFB.Ui.DiscordBot.HostedServices
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    using MassTransit;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using NFB.Domain.Bus.Events;

    /// <summary>
    /// The discord bot service.
    /// </summary>
    public class DiscordBotService : IHostedService
    {
        #region Private Fields

        /// <summary>
        /// The bus.
        /// </summary>
        private readonly IBusControl bus;

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
        /// <param name="bus">
        /// The bus.
        /// </param>
        public DiscordBotService(ILogger<DiscordBotService> logger, DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider, IConfiguration configuration, IBusControl bus)
        {
            this.logger = logger;
            this.client = client;
            this.commandService = commandService;
            this.serviceProvider = serviceProvider;
            this.bus = bus;
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
            this.client.UserVoiceStateUpdated += this.UseVoiceStatusUpdate;

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

        #region Private Methods

        /// <summary>
        /// User joined voice channel.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task UserJoinedVoiceChannel(SocketUser user, SocketVoiceState channel)
        {
            await this.bus.Publish(
                new UserJoinedVoiceChannelEvent { UserId = user.Id, ChannelId = channel.VoiceChannel.Id });
        }

        /// <summary>
        /// User left voice channel.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task UserLeftVoiceChannel(SocketUser user, SocketVoiceState channel)
        {
            await this.bus.Publish(
                new UserLeftVoiceChannelEvent { UserId = user.Id, ChannelId = channel.VoiceChannel.Id });
        }

        /// <summary>
        /// The use voice status update.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="before">
        /// The before.
        /// </param>
        /// <param name="after">
        /// The after.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task UseVoiceStatusUpdate(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (before.VoiceChannel?.Id == after.VoiceChannel?.Id)
                return; // User has deafened or muted.

            if (before.VoiceChannel != null)
                await this.UserLeftVoiceChannel(user, before);

            if (after.VoiceChannel != null)
                await this.UserJoinedVoiceChannel(user, after);
        }

        #endregion Private Methods
    }
}