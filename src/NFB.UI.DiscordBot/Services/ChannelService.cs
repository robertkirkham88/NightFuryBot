namespace NFB.UI.DiscordBot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using MongoDB.Driver;

    using NFB.UI.DiscordBot.Entities;
    using NFB.UI.DiscordBot.Models;

    /// <summary>
    /// The setup service.
    /// </summary>
    public class ChannelService : IChannelService
    {
        #region Private Fields

        /// <summary>
        /// The data.
        /// </summary>
        private readonly IMongoCollection<ChannelEntity> data;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public ChannelService(ChannelSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            this.data = database.GetCollection<ChannelEntity>(settings.CollectionName);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Create an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Create(ChannelEntity entity)
        {
            await this.data.InsertOneAsync(entity);
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/>.
        /// </returns>
        public async Task<ChannelEntity> Get(ulong id)
        {
            return await this.data.Find<ChannelEntity>(channel => channel.BookChannel == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IEnumerable<ChannelEntity>> Get()
        {
            return await this.data.Find(channel => true).ToListAsync();
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Remove(ChannelEntity entity)
        {
            await this.data.DeleteOneAsync(channel => channel.Id == entity.Id);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Update(string id, ChannelEntity entity)
        {
            await this.data.ReplaceOneAsync(channel => channel.Id == id, entity);
        }

        #endregion Public Methods
    }
}