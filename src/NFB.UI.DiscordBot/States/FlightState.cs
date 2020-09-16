namespace NFB.UI.DiscordBot.States
{
    using System;
    using System.Collections.Generic;

    using Automatonymous;

    using NFB.Domain.Bus.DTOs;

    /// <summary>
    /// The flight state.
    /// </summary>
    public class FlightState : SagaStateMachineInstance
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the correlation id.
        /// </summary>
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
        /// Gets or sets the flight active schedule token.
        /// </summary>
        public Guid? FlightActiveScheduleToken { get; set; }

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        public ulong? MessageId { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public AirportEntityDto Origin { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the users in voice channel.
        /// </summary>
        public IList<Guid> UsersInVoiceChannel { get; set; } = new List<Guid>();

        /// <summary>
        /// Gets or sets the voice channel id.
        /// </summary>
        public Guid? VoiceChannelId { get; set; }

        /// <summary>
        /// Gets or sets the voice channel ulong id.
        /// </summary>
        public ulong VoiceChannelUlongId { get; set; }

        #endregion Public Properties
    }
}