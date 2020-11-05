namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.WebSocket;

    using GreenPipes;

    using Microsoft.Extensions.Logging;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Extensions;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The delete voice channel activity.
    /// </summary>
    public class DeleteVoiceChannelActivity : Activity<FlightState, FlightCompletedEvent>
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DeleteVoiceChannelActivity> logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteVoiceChannelActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DeleteVoiceChannelActivity(DiscordSocketClient client, ILogger<DeleteVoiceChannelActivity> logger)
        {
            this.client = client;
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Accept the message.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Execute the activity.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="next">
        /// The next.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Execute(BehaviorContext<FlightState, FlightCompletedEvent> context, Behavior<FlightState, FlightCompletedEvent> next)
        {
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            if (this.client.GetChannel(context.Instance.VoiceChannelId) is SocketVoiceChannel channel && channel.Users.Count == 0)
            {
                await this.client.DeleteVoiceChannelAsync(context.Instance.VoiceChannelId);

                this.logger.LogInformation($"SAGA {context.Instance.CorrelationId}: Deleted voice channel {context.Instance.VoiceChannelId}.");
            }

            await next.Execute(context);
        }

        /// <summary>
        /// The activity faulted.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="next">
        /// The next.
        /// </param>
        /// <typeparam name="TException">
        /// The exception.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, FlightCompletedEvent, TException> context, Behavior<FlightState, FlightCompletedEvent> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }

        /// <summary>
        /// Create a scope.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Probe(ProbeContext context)
        {
            context.CreateScope("delete-voice-channel-activity");
        }

        #endregion Public Methods
    }
}