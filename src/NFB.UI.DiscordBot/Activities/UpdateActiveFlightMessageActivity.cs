namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.Rest;
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
            this.logger.LogInformation("Updating channel message for pilot data");

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
            context.Instance.UsersInVoiceChannel.Add(context.Data.UserId.ToGuid());

            await this.UpdateChannelMessage(context.Instance, true);
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
            if (context.Instance.UsersInVoiceChannel.Any(p => p == context.Data.UserId.ToGuid()))
                context.Instance.UsersInVoiceChannel.Remove(context.Data.UserId.ToGuid());

            await this.UpdateChannelMessage(context.Instance, true);
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
        /// <param name="forceUpdate">
        /// The force update.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task UpdateChannelMessage(FlightState state, bool forceUpdate = false)
        {
            var guild = this.client.Guilds.FirstOrDefault();
            if (guild == null)
                throw new InvalidOperationException("Guild cannot be null");

            if (!(guild.GetChannel(state.VoiceChannelUlongId.GetValueOrDefault()) is SocketVoiceChannel voiceChannel))
                throw new InvalidOperationException("Voice Channel cannot be null");

            if (!(guild.GetChannel(state.ChannelData.ActiveFlightMessageChannel) is SocketTextChannel activeFlightChannel))
                throw new InvalidOperationException("Active flight channel cannot be null");

            var restMessage = await activeFlightChannel.GetMessageAsync(state.ActiveFlightMessageId) as RestUserMessage;
            var socketMessage = await activeFlightChannel.GetMessageAsync(state.ActiveFlightMessageId) as SocketUserMessage;

            if (restMessage != null)
            {
                if (restMessage.EditedTimestamp.HasValue == false)
                {
                    // Edit the message
                    var embed = await FlightActiveEmbed.CreateEmbed(
                                    state.Origin,
                                    state.Destination,
                                    state.StartTime,
                                    voiceChannel,
                                    state.VatsimPilotData);

                    await restMessage.ModifyAsync(p => p.Embed = embed);
                }
                else if (restMessage.EditedTimestamp.Value < DateTimeOffset.UtcNow.AddSeconds(-45) || forceUpdate)
                {
                    // Edit the message
                    var embed = await FlightActiveEmbed.CreateEmbed(
                                    state.Origin,
                                    state.Destination,
                                    state.StartTime,
                                    voiceChannel,
                                    state.VatsimPilotData,
                                    forceUpdate ? restMessage.Embeds.First().Image : null);

                    await restMessage.ModifyAsync(p => p.Embed = embed);
                }
            }
            else if (socketMessage != null)
            {
                if (socketMessage.EditedTimestamp.HasValue == false)
                {
                    // Edit the message
                    var embed = await FlightActiveEmbed.CreateEmbed(
                                    state.Origin,
                                    state.Destination,
                                    state.StartTime,
                                    voiceChannel,
                                    state.VatsimPilotData);

                    await socketMessage.ModifyAsync(p => p.Embed = embed);
                }
                else if (socketMessage.EditedTimestamp.Value < DateTimeOffset.UtcNow.AddSeconds(-45) || forceUpdate)
                {
                    // Edit the message
                    var embed = await FlightActiveEmbed.CreateEmbed(
                                    state.Origin,
                                    state.Destination,
                                    state.StartTime,
                                    voiceChannel,
                                    state.VatsimPilotData,
                                    forceUpdate ? socketMessage.Embeds.First().Image : null);

                    await socketMessage.ModifyAsync(p => p.Embed = embed);
                }
            }
        }

        #endregion Private Methods
    }
}