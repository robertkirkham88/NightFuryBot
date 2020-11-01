namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.WebSocket;

    using GreenPipes;

    using MassTransit;

    using Microsoft.Extensions.Logging;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The flight submission failed activity.
    /// </summary>
    public class FlightSubmissionFailedActivity : Activity<FlightState, FlightInvalidEvent>
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FlightSubmissionFailedActivity> logger;

        /// <summary>
        /// The scheduler.
        /// </summary>
        private readonly IMessageScheduler scheduler;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightSubmissionFailedActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public FlightSubmissionFailedActivity(DiscordSocketClient client, IMessageScheduler scheduler, ILogger<FlightSubmissionFailedActivity> logger)
        {
            this.client = client;
            this.scheduler = scheduler;
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
        /// Execute the context.
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
        public async Task Execute(BehaviorContext<FlightState, FlightInvalidEvent> context, Behavior<FlightState, FlightInvalidEvent> next)
        {
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            if (this.client.GetChannel(context.Instance.ChannelData.BookChannel) is SocketTextChannel channel)
            {
                var message = await channel.SendMessageAsync(context.Data.ValidationError);

                await this.scheduler.SchedulePublish(
                    DateTime.UtcNow.AddSeconds(30),
                    new DiscordMessageExpiredEvent { ChannelId = message.Channel.Id, MessageId = message.Id });
            }

            await next.Execute(context);
        }

        /// <summary>
        /// The activity has faulted.
        /// </summary>
        /// <typeparam name="TException">
        /// The exception type.
        /// </typeparam>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="next">
        /// The next.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, FlightInvalidEvent, TException> context, Behavior<FlightState, FlightInvalidEvent> next)
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
            context.CreateScope("flight-invalid");
        }

        #endregion Public Methods
    }
}