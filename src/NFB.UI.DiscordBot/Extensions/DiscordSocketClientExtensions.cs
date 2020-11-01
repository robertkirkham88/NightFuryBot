namespace NFB.UI.DiscordBot.Extensions
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    using Discord;
    using Discord.Rest;
    using Discord.WebSocket;

    /// <summary>
    /// The discord socket client extensions.
    /// </summary>
    public static class DiscordSocketClientExtensions
    {
        #region Public Methods

        /// <summary>
        /// Create a new voice channel in the parent category.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="categoryId">
        /// The parent id.
        /// </param>
        /// <param name="channelName">
        /// The channel name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<RestVoiceChannel> CreateVoiceChannelAsync(this DiscordSocketClient client, ulong categoryId, string channelName)
        {
            return await client.Guilds.First()
                       .CreateVoiceChannelAsync(
                           channelName,
                           properties => { properties.CategoryId = categoryId; });
        }

        /// <summary>
        /// Remove a message from a channel.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="channelId">
        /// The channel id.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task DeleteMessageFromChannelAsync(this DiscordSocketClient client, ulong channelId, ulong messageId)
        {
            if (client.GetChannel(channelId) is SocketTextChannel channel)
            {
                var message = await channel.GetMessageAsync(messageId);
                await message.DeleteAsync();
            }
        }

        /// <summary>
        /// Remove a voice channel.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="channelId">
        /// The channel ID.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<bool> DeleteVoiceChannelAsync(this DiscordSocketClient client, ulong channelId)
        {
            if (!(client.GetChannel(channelId) is SocketVoiceChannel channel) || channel.Users.Any()) return false;

            await channel.DeleteAsync();
            return true;
        }

        /// <summary>
        /// Get channel with predicate.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="pred">
        /// The pred.
        /// </param>
        /// <returns>
        /// The <see cref="SocketGuildChannel"/>.
        /// </returns>
        public static SocketGuildChannel GetChannel(this DiscordSocketClient client, Func<SocketGuildChannel, bool> pred)
        {
            var socketGuildChannels = client.Guilds.First().Channels;

            return socketGuildChannels?.FirstOrDefault(pred);
        }

        /// <summary>
        /// Send a message to a specific channel.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="channelId">
        /// The channel id.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="isTTS">
        /// The TTS.
        /// </param>
        /// <param name="embed">
        /// The embed.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<RestUserMessage> SendMessageToChannelAsync(
            this DiscordSocketClient client,
            ulong channelId,
            string text = null,
            bool isTTS = false,
            Discord.Embed embed = null,
            RequestOptions options = null)
        {
            if (!(client.GetChannel(channelId) is SocketTextChannel channel)) throw new ArgumentNullException($"SocketTextChannel with ID {channelId} not found.");

            return await channel.SendMessageAsync(text, isTTS, embed, options);
        }

        /// <summary>
        /// Update a message in a channel.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="channelId">
        /// The channel id.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="minimumPreviousEditTimeSpan">
        /// The time in which the message cannot be edited.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="embed">
        /// The embed.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task UpdateMessageInChannelAsync(
            this DiscordSocketClient client,
            ulong channelId,
            ulong messageId,
            TimeSpan? minimumPreviousEditTimeSpan = null,
            string text = "",
            Discord.Embed embed = null)
        {
            if (!(client.GetChannel(channelId) is SocketTextChannel channel)) throw new ArgumentNullException($"SocketTextChannel with ID {channelId} not found.");
            var restMessage = await channel.GetMessageAsync(messageId) as RestUserMessage;
            var socketMessage = await channel.GetMessageAsync(messageId) as SocketUserMessage;

            if (restMessage == null && socketMessage == null) throw new ArgumentNullException($"Unable to find a message with ID {messageId} in channel {channelId}.");

            if (restMessage != null)
            {
                if (minimumPreviousEditTimeSpan.HasValue && restMessage.EditedTimestamp.HasValue)
                {
                    if (DateTime.UtcNow < restMessage.EditedTimestamp.GetValueOrDefault()
                            .Add(minimumPreviousEditTimeSpan.GetValueOrDefault()))
                        return;
                }

                await restMessage.ModifyAsync(
                    p =>
                        {
                            p.Embed = embed;
                            p.Content = text;
                        });
                return;
            }

            if (minimumPreviousEditTimeSpan.HasValue && socketMessage.EditedTimestamp.HasValue)
            {
                if (DateTime.UtcNow < socketMessage.EditedTimestamp.GetValueOrDefault()
                        .Add(minimumPreviousEditTimeSpan.GetValueOrDefault()))
                    return;
            }

            await socketMessage.ModifyAsync(
                p =>
                    {
                        p.Embed = embed;
                        p.Content = text;
                    });
        }

        #endregion Public Methods
    }
}