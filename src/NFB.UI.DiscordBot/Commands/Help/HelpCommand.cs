namespace NFB.UI.DiscordBot.Commands.Help
{
    using System.Linq;
    using System.Threading.Tasks;

    using Discord;
    using Discord.Commands;

    /// <summary>
    /// The help command.
    /// </summary>
    [Group("Help")]
    [Alias("h")]
    public class HelpCommand : ModuleBase
    {
        #region Private Fields

        /// <summary>
        /// The command service.
        /// </summary>
        private readonly CommandService commandService;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommand"/> class.
        /// </summary>
        /// <param name="commandService">
        /// The command Service.
        /// </param>
        public HelpCommand(CommandService commandService)
        {
            this.commandService = commandService;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Command]
        [Name("Help")]
        [Summary("Display a list of commands with descriptions.\r\nExample: !help")]
#pragma warning disable 1998
        public async Task<RuntimeResult> ExecuteAsync()
#pragma warning restore 1998
        {
            var commands = this.commandService.Commands.ToList();
            var embedBuilder = new EmbedBuilder { Title = "Help", Description = "List of available commands", Color = Color.DarkGreen };

            foreach (var command in commands)
            {
                var summary = command.Summary ?? "No description available";

                embedBuilder.AddField(command.Name, $"{summary}");
            }

            return CommandResult.FromSuccess(directMessage: new CommandResultMessage(embed: embedBuilder.Build()));
        }

        #endregion Public Methods
    }
}