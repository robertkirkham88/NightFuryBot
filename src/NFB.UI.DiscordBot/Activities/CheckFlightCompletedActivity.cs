namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.Rest;
    using Discord.WebSocket;

    using GreenPipes;

    using NFB.Domain.Bus.Events;
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

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckFlightCompletedActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public CheckFlightCompletedActivity(DiscordSocketClient client)
        {
            this.client = client;
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
            var voiceChannel = this.client.GetChannel(context.Instance.VoiceChannelUlongId.GetValueOrDefault()) as SocketVoiceChannel;
            var activeChannel = this.client.GetChannel(context.Instance.ChannelData.ActiveFlightMessageChannel) as SocketTextChannel;

            if (voiceChannel != null && voiceChannel.Users.Any())
            {
                await next.Execute(context);
                return;
            }

            if (activeChannel != null)
            {
                var restMessage = await activeChannel.GetMessageAsync(context.Instance.ActiveFlightMessageId) as RestUserMessage;
                var socketMessage = await activeChannel.GetMessageAsync(context.Instance.ActiveFlightMessageId) as SocketUserMessage;
                restMessage?.DeleteAsync();
                socketMessage?.DeleteAsync();
            }

            if (voiceChannel != null)
                await voiceChannel.DeleteAsync();

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