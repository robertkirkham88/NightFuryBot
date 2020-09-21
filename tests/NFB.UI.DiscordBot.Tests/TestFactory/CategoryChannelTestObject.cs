namespace NFB.UI.DiscordBot.Tests.TestFactory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Discord;

    /// <summary>
    /// The category channel test object.
    /// </summary>
    public class CategoryChannelTestObject : ICategoryChannel
    {
        #region Public Properties

        public IList<ITextChannel> Channels { get; set; } = new List<ITextChannel>();
        public DateTimeOffset CreatedAt { get; }
        public IGuild Guild { get; }
        public ulong GuildId { get; }
        public ulong Id { get; }
        public string Name { get; set; }

        public IReadOnlyCollection<Overwrite> PermissionOverwrites { get; }

        public int Position { get; }

        #endregion Public Properties

        #region Public Methods

        public Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            throw new NotImplementedException();
        }

        public OverwritePermissions? GetPermissionOverwrite(IUser user)
        {
            throw new NotImplementedException();
        }

        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            throw new NotImplementedException();
        }

        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        {
            return this.GetUsersAsync(mode, options);
        }

        public Task ModifyAsync(Action<GuildChannelProperties> func, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}