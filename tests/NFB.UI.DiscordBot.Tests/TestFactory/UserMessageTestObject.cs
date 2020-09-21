namespace NFB.UI.DiscordBot.Tests.TestFactory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Discord;

    public class UserMessageTestObject : IUserMessage
    {
        #region Public Properties

        public MessageActivity Activity { get; }
        public MessageApplication Application { get; }
        public IReadOnlyCollection<IAttachment> Attachments { get; }
        public IUser Author { get; }
        public IMessageChannel Channel { get; }
        public string Content { get; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset? EditedTimestamp { get; }
        public IReadOnlyCollection<IEmbed> Embeds { get; }
        public ulong Id { get; set; }
        public bool IsPinned { get; }

        public bool IsSuppressed { get; }

        public bool IsTTS { get; }

        public IReadOnlyCollection<ulong> MentionedChannelIds { get; }

        public IReadOnlyCollection<ulong> MentionedRoleIds { get; }

        public IReadOnlyCollection<ulong> MentionedUserIds { get; }

        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions { get; }

        public MessageReference Reference { get; }

        public MessageSource Source { get; }

        public IReadOnlyCollection<ITag> Tags { get; }

        public DateTimeOffset Timestamp { get; }

        public MessageType Type { get; }

        #endregion Public Properties

        #region Public Methods

        public Task AddReactionAsync(IEmote emote, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task ModifyAsync(Action<MessageProperties> func)
        {
            throw new NotImplementedException();
        }

        public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task ModifySuppressionAsync(bool suppressEmbeds, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task PinAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllReactionsAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public string Resolve(
            TagHandling userHandling = TagHandling.Name,
            TagHandling channelHandling = TagHandling.Name,
            TagHandling roleHandling = TagHandling.Name,
            TagHandling everyoneHandling = TagHandling.Ignore,
            TagHandling emojiHandling = TagHandling.Name)
        {
            throw new NotImplementedException();
        }

        public Task UnpinAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}