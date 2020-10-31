namespace NFB.UI.DiscordBot.Consumers
{
    using System.Threading.Tasks;

    using Discord.WebSocket;

    using MassTransit;

    using NFB.Domain.Bus.Events;

    /// <summary>
    /// The discord message expired consumer.
    /// </summary>
    public class DiscordMessageExpiredConsumer : IConsumer<DiscordMessageExpiredEvent>
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordMessageExpiredConsumer"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public DiscordMessageExpiredConsumer(DiscordSocketClient client)
        {
            this.client = client;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The consume.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Consume(ConsumeContext<DiscordMessageExpiredEvent> context)
        {
            if (this.client.GetChannel(context.Message.ChannelId) is SocketTextChannel textChannel)
            {
                var message = await textChannel.GetMessageAsync(context.Message.MessageId);
                if (message != null)
                    await message.DeleteAsync();
            }
        }

        #endregion Public Methods
    }
}