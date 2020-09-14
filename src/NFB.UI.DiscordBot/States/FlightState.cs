namespace NFB.UI.DiscordBot.States
{
    using System;

    using Automatonymous;

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
        /// Gets or sets the flight active schedule token.
        /// </summary>
        public Guid? FlightActiveScheduleToken { get; set; }

        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        public ulong? MessageId { get; set; }

        /// <summary>
        /// Gets or sets the voice channel id.
        /// </summary>
        public ulong? VoiceChannelId { get; set; }

        #endregion Public Properties
    }
}