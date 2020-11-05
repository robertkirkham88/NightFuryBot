namespace NFB.UI.DiscordBot.StateMachines.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.WebSocket;

    using GreenPipes;

    using Microsoft.Extensions.Logging;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Embeds;
    using NFB.UI.DiscordBot.Extensions;
    using NFB.UI.DiscordBot.Schedules;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The update active flight message.
    /// </summary>
    public class UpdateActiveFlightMessageActivity : Activity<FlightState, FlightStartingEvent>, Activity<FlightState, UserJoinedVoiceChannelEvent>, Activity<FlightState, UserLeftVoiceChannelEvent>, Activity<FlightState, UpdatePilotDataScheduleMessage>, Activity<FlightState, FlightCreatedEvent>
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UpdateActiveFlightMessageActivity> logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateActiveFlightMessageActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UpdateActiveFlightMessageActivity(DiscordSocketClient client, ILogger<UpdateActiveFlightMessageActivity> logger)
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
        public async Task Execute(BehaviorContext<FlightState, FlightStartingEvent> context, Behavior<FlightState, FlightStartingEvent> next)
        {
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            await this.UpdateChannelMessage(context.Instance);

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
        public async Task Execute(BehaviorContext<FlightState, UserJoinedVoiceChannelEvent> context, Behavior<FlightState, UserJoinedVoiceChannelEvent> next)
        {
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            if (context.Instance.UsersInVoiceChannel.Any(p => p == context.Data.UserId) == false)
                context.Instance.UsersInVoiceChannel.Add(context.Data.UserId);

            await this.UpdateChannelMessage(context.Instance, TimeSpan.FromSeconds(0));
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
        public async Task Execute(BehaviorContext<FlightState, UserLeftVoiceChannelEvent> context, Behavior<FlightState, UserLeftVoiceChannelEvent> next)
        {
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            if (context.Instance.UsersInVoiceChannel.Any(p => p == context.Data.UserId))
                context.Instance.UsersInVoiceChannel.Remove(context.Data.UserId);

            await this.UpdateChannelMessage(context.Instance, TimeSpan.FromSeconds(0));
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
        public async Task Execute(BehaviorContext<FlightState, UpdatePilotDataScheduleMessage> context, Behavior<FlightState, UpdatePilotDataScheduleMessage> next)
        {
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            await this.UpdateChannelMessage(context.Instance);
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
        public async Task Execute(BehaviorContext<FlightState, FlightCreatedEvent> context, Behavior<FlightState, FlightCreatedEvent> next)
        {
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            await this.UpdateChannelMessage(context.Instance);
            await next.Execute(context);
        }

        /// <summary>
        /// Activity faulted.
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
        public Task Faulted<TException>(BehaviorExceptionContext<FlightState, FlightStartingEvent, TException> context, Behavior<FlightState, FlightStartingEvent> next)
                    where TException : Exception
        {
            return next.Faulted(context);
        }

        /// <summary>
        /// Activity faulted.
        /// </summary>
        /// <typeparam name="TException">
        /// The exception.
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
        public Task Faulted<TException>(BehaviorExceptionContext<FlightState, UserJoinedVoiceChannelEvent, TException> context, Behavior<FlightState, UserJoinedVoiceChannelEvent> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        /// <summary>
        /// Activity faulted.
        /// </summary>
        /// <typeparam name="TException">
        /// The exception.
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
        public Task Faulted<TException>(BehaviorExceptionContext<FlightState, UserLeftVoiceChannelEvent, TException> context, Behavior<FlightState, UserLeftVoiceChannelEvent> next)
                    where TException : Exception
        {
            return next.Faulted(context);
        }

        /// <summary>
        /// Activity faulted.
        /// </summary>
        /// <typeparam name="TException">
        /// The exception.
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
        public Task Faulted<TException>(BehaviorExceptionContext<FlightState, UpdatePilotDataScheduleMessage, TException> context, Behavior<FlightState, UpdatePilotDataScheduleMessage> next)
                    where TException : Exception
        {
            return next.Faulted(context);
        }

        /// <summary>
        /// Activity faulted.
        /// </summary>
        /// <typeparam name="TException">
        /// The exception.
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
        public Task Faulted<TException>(BehaviorExceptionContext<FlightState, FlightCreatedEvent, TException> context, Behavior<FlightState, FlightCreatedEvent> next)
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
            context.CreateScope("update-active-flight-message");
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Update the channel message.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="minimumPreviousEditTimeSpan">
        /// The minimum Previous Edit Time Span.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task UpdateChannelMessage(FlightState state, TimeSpan? minimumPreviousEditTimeSpan = null)
        {
            if (minimumPreviousEditTimeSpan == null)
                minimumPreviousEditTimeSpan = TimeSpan.FromMinutes(1);

            if (!(this.client.GetChannel(state.VoiceChannelId) is SocketVoiceChannel voiceChannel))
                throw new ArgumentNullException($"Voice channel with ID {state.VoiceChannelId} not found.");

            this.logger.LogInformation($"SAGA {state.CorrelationId}: Updating {state.ActiveFlightMessageId} with {minimumPreviousEditTimeSpan.GetValueOrDefault().TotalSeconds}sec time constraint.");

            await this.client.UpdateMessageInChannelAsync(
                state.ChannelData.ActiveFlightMessageChannel,
                state.ActiveFlightMessageId,
                minimumPreviousEditTimeSpan,
                embed: await FlightActiveEmbed.CreateEmbed(
                           state.Origin,
                           state.Destination,
                           state.StartTime,
                           voiceChannel,
                           state.VatsimPilotData));
        }

        #endregion Private Methods
    }
}