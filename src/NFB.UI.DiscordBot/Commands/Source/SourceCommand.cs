namespace NFB.UI.DiscordBot.Commands.Source
{
    using System.Threading.Tasks;

    using Discord.Commands;

    /// <summary>
    /// The source command.
    /// </summary>
    [Group("Source")]
    [Alias("s")]
    public class SourceCommand : ModuleBase
    {
        #region Public Methods

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Command]
        [Name("Source")]
        [Summary("Show the source code for the bot.\r\nExample: !source")]
#pragma warning disable 1998
        public async Task<RuntimeResult> ExecuteAsync()
        {
            return CommandResult.FromSuccess(
                new CommandResultMessage(
                    "So, you want to see how I work, eh? Visit: https://github.com/robertkirkham88/NightFuryBot"));
        }

        #endregion Public Methods

#pragma warning restore 1998
    }
}