namespace NFB.Ui.DiscordBot.HostedServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using NFB.UI.DiscordBot.Commands;

    /// <summary>
    /// The discord command service.
    /// </summary>
    public class DiscordCommandService : IHostedService
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
        private readonly ILogger<DiscordCommandService> logger;

        /// <summary>
        /// The service provider.
        /// </summary>
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// The token.
        /// </summary>
        private CancellationToken token;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordCommandService"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="commandService">
        /// The command Service.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="serviceProvider">
        /// The service Provider.
        /// </param>
        public DiscordCommandService(ILogger<DiscordCommandService> logger, CommandService commandService, DiscordSocketClient client, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.commandService = commandService;
            this.client = client;
            this.serviceProvider = serviceProvider;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Start the command service.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.token = cancellationToken;

            this.client.MessageReceived += this.OnMessageReceived;
            this.commandService.CommandExecuted += this.ParseCommandResultAsync;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stop the command service.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.client.MessageReceived -= this.OnMessageReceived;
            this.commandService.CommandExecuted -= this.ParseCommandResultAsync;

            return Task.CompletedTask;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Execute a command that is passed by the user.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task ExecuteAsync(ICommandContext context, IServiceProvider provider, string input)
        {
            var result = await this.commandService.ExecuteAsync(context, input, provider);

            // Unknown command or successful, do nothing.
            if (result.IsSuccess || result.Error == CommandError.UnknownCommand)
                return;

            if (result is ParseResult parse && parse.Error == CommandError.BadArgCount)
            {
                // The command has the incorrect number of arguments.
            }

            if (result.Error == CommandError.UnmetPrecondition)
            {
                // Not enough access to run command.
            }
        }

        /// <summary>
        /// Check if the input is a command.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="argPos">
        /// The prefix position.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsCommand(ICommandContext context, string prefix, out int argPos)
        {
            argPos = 0;
            if (context.User.Id == this.client.CurrentUser.Id) return false;
            var hasStringPrefix = prefix != null && context.Message.HasStringPrefix(prefix, ref argPos);
            return hasStringPrefix || context.Message.HasMentionPrefix(this.client.CurrentUser, ref argPos);
        }

        /// <summary>
        /// Execute when a message has been received.
        /// </summary>
        /// <param name="s">
        /// The socket message.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private Task OnMessageReceived(SocketMessage s)
        {
            Task.Run(
                async () =>
                    {
                        if (!(s is SocketUserMessage msg))
                            return;

                        var context = new CommandContext(this.client, msg);
                        const string Prefix = "!"; // Commands start with !

                        if (this.IsCommand(context, Prefix, out var argPos))
                        {
                            this.logger.LogInformation($"{context.Message.Author} executed '{context.Message.Content.Substring(argPos)}'");
                            await this.ExecuteAsync(context, this.serviceProvider, context.Message.Content.Substring(argPos));
                        }
                    },
                this.token);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Parse the command result that has just been executed.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task ParseCommandResultAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (result is CommandResult parseResult)
            {
                // Direct message
                if (parseResult.DirectMessage != null)
                    await context.User.SendMessageAsync(parseResult.DirectMessage.Message ?? string.Empty, embed: parseResult.DirectMessage.Embed);

                // Context message.
                if (parseResult.ContextMessage != null && (context.Channel.GetType().ToString() == "Discord.WebSocket.SocketTextChannel" || parseResult.DirectMessage == null))
                    await context.Channel.SendMessageAsync(parseResult.ContextMessage.Message ?? string.Empty, embed: parseResult.ContextMessage.Embed);

                if (parseResult.Error != null)
                    this.logger.LogWarning($"'{command.Value.Name}' Error for {context.Message.Author}: {parseResult.LogMessage ?? "(No log message)"}");
                else
                    this.logger.LogInformation(
                        string.IsNullOrEmpty(parseResult.Reason)
                            ? $"'{command.Value.Name}' Successful for {context.Message.Author}."
                            : $"'{command.Value.Name}' Successful for {context.Message.Author}: {parseResult.LogMessage}");
            }
        }

        #endregion Private Methods
    }
}