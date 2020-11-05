namespace NFB.UI.DiscordBot.StateMachines.Activities
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
    /// The delete active flight message activity.
    /// </summary>
    public class DeleteActiveFlightMessageActivity : Activity<FlightState, FlightCompletedEvent>, Activity<FlightState, VoiceChannelRemovedEvent>
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<DeleteActiveFlightMessageActivity> logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteActiveFlightMessageActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DeleteActiveFlightMessageActivity(DiscordSocketClient client, ILogger<DeleteActiveFlightMessageActivity> logger)
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

            if (context.Instance.ActiveFlightMessageId != default)
            {
                await this.client.DeleteMessageFromChannelAsync(
                    context.Instance.ChannelData.ActiveFlightMessageChannel,
                    context.Instance.ActiveFlightMessageId);

                context.Instance.ActiveFlightMessageId = default;

                this.logger.LogInformation($"SAGA {context.Instance.CorrelationId}: Deleted message {context.Instance.ActiveFlightMessageId} in {context.Instance.ChannelData.ActiveFlightMessageChannel}.");
            }

            await next.Execute(context);
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
        public async Task Execute(BehaviorContext<FlightState, VoiceChannelRemovedEvent> context, Behavior<FlightState, VoiceChannelRemovedEvent> next)
        {
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            if (context.Instance.ActiveFlightMessageId != default)
            {
                await this.client.DeleteMessageFromChannelAsync(
                    context.Instance.ChannelData.ActiveFlightMessageChannel,
                    context.Instance.ActiveFlightMessageId);

                context.Instance.ActiveFlightMessageId = default;

                this.logger.LogInformation($"SAGA {context.Instance.CorrelationId}: Deleted message {context.Instance.ActiveFlightMessageId} in {context.Instance.ChannelData.ActiveFlightMessageChannel}.");
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
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, VoiceChannelRemovedEvent, TException> context, Behavior<FlightState, VoiceChannelRemovedEvent> next)
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
            context.CreateScope("delete-active-flight-message");
        }

        #endregion Public Methods
    }
}