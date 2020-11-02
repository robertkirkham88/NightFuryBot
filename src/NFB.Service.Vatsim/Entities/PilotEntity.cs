namespace NFB.Service.Vatsim.Entities
{
    using System;

    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// The vatsim entity.
    /// </summary>
    public class PilotEntity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [BsonId]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the vatsim id.
        /// </summary>
        public string VatsimId { get; set; }

        #endregion Public Properties
    }
}