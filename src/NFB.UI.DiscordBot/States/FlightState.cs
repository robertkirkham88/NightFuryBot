namespace NFB.UI.DiscordBot.States
{
    using System;
    using System.Collections.Generic;

    using Automatonymous;

    using MassTransit.Saga;

    using MongoDB.Bson.Serialization.Attributes;

    using NFB.Domain.Bus.DTOs;
    using NFB.UI.DiscordBot.Models;

    /// <summary>
    /// The flight state.
    /// </summary>
    public class FlightState : SagaStateMachineInstance, ISagaVersion
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the announcement channel id.
        /// </summary>
        public ulong AnnouncementChannelId { get; set; }

        /// <summary>
        /// Gets or sets the available colors.
        /// </summary>
        public IList<uint> AvailableColors { get; set; } = new List<uint>();

        /// <summary>
        /// Gets or sets the category id.
        /// </summary>
        public ulong CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the check flight completed token.
        /// </summary>
        public Guid? CheckFlightCompletedToken { get; set; }

        /// <summary>
        /// Gets or sets the correlation id.
        /// </summary>
        [BsonId]
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the current state.
        /// </summary>
        public string CurrentState { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        public AirportEntityDto Destination { get; set; }

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        public ulong? MessageId { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public AirportEntityDto Origin { get; set; }

        /// <summary>
        /// Gets or sets the original category id.
        /// </summary>
        public ulong RequestCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the request channel id.
        /// </summary>
        public ulong RequestChannelId { get; set; }

        /// <summary>
        /// Gets or sets the original request.
        /// </summary>
        public ulong RequestMessageId { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the update pilot data in message token.
        /// </summary>
        public Guid? UpdatePilotDataInMessageToken { get; set; }

        /// <summary>
        /// Gets or sets the users in voice channel.
        /// </summary>
        public IList<Guid> UsersInVoiceChannel { get; set; } = new List<Guid>();

        /// <summary>
        /// Gets or sets the vatsim pilot data.
        /// </summary>
        public IList<VatsimPilotModel> VatsimPilotData { get; set; } = new List<VatsimPilotModel>();

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the voice channel id.
        /// </summary>
        public Guid? VoiceChannelId { get; set; }

        /// <summary>
        /// Gets or sets the voice channel ulong id.
        /// </summary>
        public ulong? VoiceChannelUlongId { get; set; }

        #endregion Public Properties
    }
}