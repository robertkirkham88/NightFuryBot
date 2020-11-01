namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.WebSocket;

    using GreenPipes;

    using Microsoft.Extensions.Logging;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Exceptions;
    using NFB.UI.DiscordBot.Extensions;
    using NFB.UI.DiscordBot.Schedules;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The update voice channel users activity.
    /// </summary>
    public class CheckFlightCompletedActivity : Activity<FlightState, CheckFlightCompletedScheduleMessage>
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckFlightCompletedActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public CheckFlightCompletedActivity(DiscordSocketClient client, ILogger<CheckFlightCompletedActivity> logger)
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
        public async Task Execute(BehaviorContext<FlightState, CheckFlightCompletedScheduleMessage> context, Behavior<FlightState, CheckFlightCompletedScheduleMessage> next)
        {
            try
            {
                // If the voice channel has been removed successfully.
                if (await this.client.DeleteVoiceChannelAsync(context.Instance.VoiceChannelUlongId.GetValueOrDefault()))
                {
                    await this.client.DeleteMessageFromChannelAsync(
                        context.Instance.ChannelData.ActiveFlightMessageChannel,
                        context.Instance.ActiveFlightMessageId);
                }
                else
                {
                    await next.Execute(context);
                    return;
                }
            }
            catch (VoiceChannelNotEmptyException)
            {
                return;
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex.ToString());
            }

            await context.Publish(new FlightCompletedEvent { Id = context.Instance.CorrelationId });
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
        public Task Faulted<TException>(BehaviorExceptionContext<FlightState, CheckFlightCompletedScheduleMessage, TException> context, Behavior<FlightState, CheckFlightCompletedScheduleMessage> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        /// <summary>
        /// Create a scope.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Probe(ProbeContext context)
        {
            context.CreateScope("check-flight-completed");
        }

        #endregion Public Methods
    }
}