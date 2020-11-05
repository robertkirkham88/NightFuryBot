namespace NFB.UI.DiscordBot.Commands.Setup
{
    using System.Threading.Tasks;

    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    using NFB.UI.DiscordBot.Entities;
    using NFB.UI.DiscordBot.Repositories;

    /// <summary>
    /// The setup command.
    /// </summary>
    [Group("Setup")]
    [Alias("s")]
    [RequireUserPermission(GuildPermission.ManageChannels)]
    public class SetupCommand : ModuleBase
    {
        #region Private Fields

        /// <summary>
        /// The channel service.
        /// </summary>
        private readonly IChannelRepository channelService;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupCommand"/> class.
        /// </summary>
        /// <param name="channelService">
        /// The channel service.
        /// </param>
        public SetupCommand(IChannelRepository channelService)
        {
            this.channelService = channelService;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Setup actions for a category.
        /// </summary>
        /// <param name="announcementChannel">
        /// The announcement Channel.
        /// </param>
        /// <param name="activeFlightChannel">
        /// The channel where the active flight message is going to be located.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Command]
        [Name("Setup")]
        public async Task<RuntimeResult> ExecuteAsync(SocketTextChannel announcementChannel, SocketTextChannel activeFlightChannel)
        {
            var existingChannel = await this.channelService.Get(this.Context.Channel.Id);

            if (existingChannel == null)
            {
                // new channel
                var entity = new ChannelEntity
                {
                    BookChannel = this.Context.Channel.Id,
                    AnnouncementChannel = announcementChannel.Id,
                    Category = announcementChannel.Category.Id,
                    ActiveFlightMessageChannel = activeFlightChannel.Id
                };

                await this.channelService.Create(entity);
            }
            else
            {
                existingChannel.AnnouncementChannel = announcementChannel.Id;
                existingChannel.Category = announcementChannel.Category.Id;
                existingChannel.ActiveFlightMessageChannel = activeFlightChannel.Id;

                await this.channelService.Update(existingChannel.Id, existingChannel);
            }

            return CommandResult.FromSuccess(
                $"Successfully set the announcement channel as {announcementChannel.Name}, active flight message channel as {activeFlightChannel.Name}.");
        }

        #endregion Public Methods
    }
}