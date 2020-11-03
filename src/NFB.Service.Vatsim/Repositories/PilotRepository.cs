namespace NFB.Service.Vatsim.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using MongoDB.Driver;

    using NFB.Service.Vatsim.Entities;
    using NFB.Service.Vatsim.Models;

    /// <summary>
    /// The setup service.
    /// </summary>
    public class PilotRepository : IPilotRepository
    {
        #region Private Fields

        /// <summary>
        /// The data.
        /// </summary>
        private readonly IMongoCollection<PilotEntity> data;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotRepository"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public PilotRepository(MongoSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            this.data = database.GetCollection<PilotEntity>("pilot-entities");
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
        public async Task Create(PilotEntity entity)
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
        public async Task<PilotEntity> Get(string id)
        {
            return await this.data.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="predicate">
        ///     The predicate.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<List<PilotEntity>> Get(Expression<Func<PilotEntity, bool>> predicate)
        {
            return await this.data.Find(predicate).ToListAsync();
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IEnumerable<PilotEntity>> Get()
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
        public async Task Remove(PilotEntity entity)
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
        public async Task Update(string id, PilotEntity entity)
        {
            await this.data.ReplaceOneAsync(p => p.Id == id, entity);
        }

        #endregion Public Methods
    }
}