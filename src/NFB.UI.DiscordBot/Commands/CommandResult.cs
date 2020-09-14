namespace NFB.UI.DiscordBot.Commands
{
    using Discord.Commands;

    /// <summary>
    /// The command result.
    /// </summary>
    public class CommandResult : RuntimeResult
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult"/> class.
        /// </summary>
        /// <param name="contextMessage">
        /// The context Message.
        /// </param>
        /// <param name="directMessage">
        /// The direct Message.
        /// </param>
        /// <param name="logMessage">
        /// The log message.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public CommandResult(CommandResultMessage contextMessage = null, CommandResultMessage directMessage = null, string logMessage = null, CommandError? error = null)
            : base(error, contextMessage?.Message ?? directMessage?.Message)
        {
            this.ContextMessage = contextMessage;
            this.DirectMessage = directMessage;
            this.LogMessage = logMessage;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the context message.
        /// </summary>
        public CommandResultMessage ContextMessage { get; }

        /// <summary>
        /// Gets the direct message.
        /// </summary>
        public CommandResultMessage DirectMessage { get; }

        /// <summary>
        /// Gets the log message.
        /// </summary>
        public string LogMessage { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Create a new command result from error.
        /// </summary>
        /// <param name="contextMessage">
        /// The message to send to the context.
        /// </param>
        /// <param name="directMessage">
        /// The direct message to the user.
        /// </param>
        /// <param name="logMessage">
        /// The log Message.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="CommandResult"/>.
        /// </returns>
        public static CommandResult FromError(CommandResultMessage contextMessage = null, CommandResultMessage directMessage = null, string logMessage = null, CommandError error = CommandError.Unsuccessful) => new CommandResult(contextMessage, directMessage, logMessage, error);

        /// <summary>
        /// Create a new command result from error.
        /// </summary>
        /// <param name="contextMessage">
        /// The message to send to the context.
        /// </param>
        /// <param name="directMessage">
        /// The direct message to the user.
        /// </param>
        /// <param name="logMessage">
        /// The log Message.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// The <see cref="CommandResult"/>.
        /// </returns>
        public static CommandResult FromError(string contextMessage = null, string directMessage = null, string logMessage = null, CommandError error = CommandError.Unsuccessful) => new CommandResult(string.IsNullOrEmpty(contextMessage) ? null : new CommandResultMessage(contextMessage), string.IsNullOrEmpty(directMessage) ? null : new CommandResultMessage(directMessage), logMessage, error);

        /// <summary>
        /// Create a new command result from success.
        /// </summary>
        /// <param name="contextMessage">
        /// The message to send to the context.
        /// </param>
        /// <param name="directMessage">
        /// The direct message to the user.
        /// </param>
        /// <param name="logMessage">
        /// The log message.
        /// </param>
        /// <returns>
        /// The <see cref="CommandResult"/>.
        /// </returns>
        public static CommandResult FromSuccess(CommandResultMessage contextMessage = null, CommandResultMessage directMessage = null, string logMessage = null) => new CommandResult(contextMessage, directMessage, logMessage);

        /// <summary>
        /// Create a new command result from success.
        /// </summary>
        /// <param name="contextMessage">
        /// The message to send to the context.
        /// </param>
        /// <param name="directMessage">
        /// The direct message to the user.
        /// </param>
        /// <param name="logMessage">
        /// The log message.
        /// </param>
        /// <returns>
        /// The <see cref="CommandResult"/>.
        /// </returns>
        public static CommandResult FromSuccess(string contextMessage = null, string directMessage = null, string logMessage = null) => new CommandResult(string.IsNullOrEmpty(contextMessage) ? null : new CommandResultMessage(contextMessage), string.IsNullOrEmpty(directMessage) ? null : new CommandResultMessage(directMessage), logMessage);

        #endregion Public Methods
    }
}