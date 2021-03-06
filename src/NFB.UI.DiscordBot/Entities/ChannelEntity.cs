﻿namespace NFB.UI.DiscordBot.Entities
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// The channel entity.
    /// </summary>
    public class ChannelEntity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the active flight channel.
        /// </summary>
        public ulong ActiveFlightMessageChannel { get; set; }

        /// <summary>
        /// Gets or sets the announcement channel.
        /// </summary>
        public ulong AnnouncementChannel { get; set; }

        /// <summary>
        /// Gets or sets the channel where the flight has been booked.
        /// </summary>
        public ulong BookChannel { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public ulong Category { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        #endregion Public Properties
    }
}