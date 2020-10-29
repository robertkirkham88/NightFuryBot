namespace NFB.UI.DiscordBot.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using NFB.UI.DiscordBot.Entities;

    /// <summary>
    /// The SetupService interface.
    /// </summary>
    public interface IChannelService
    {
        #region Public Methods

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Create(ChannelEntity entity);

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<ChannelEntity> Get(ulong id);

        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IEnumerable<ChannelEntity>> Get();

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task Remove(ChannelEntity entity);

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
        Task Update(string id, ChannelEntity entity);

        #endregion Public Methods
    }
}