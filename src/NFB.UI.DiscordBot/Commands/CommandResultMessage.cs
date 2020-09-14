namespace NFB.UI.DiscordBot.Commands
{
    using Discord;

    /// <summary>
    /// The command result message.
    /// </summary>
    public class CommandResultMessage
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResultMessage"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="embed">
        /// The embed.
        /// </param>
        public CommandResultMessage(string message = null, Embed embed = null)
        {
            this.Message = message;
            this.Embed = embed;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the embed.
        /// </summary>
        public Embed Embed { get; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; }

        #endregion Public Properties
    }
}