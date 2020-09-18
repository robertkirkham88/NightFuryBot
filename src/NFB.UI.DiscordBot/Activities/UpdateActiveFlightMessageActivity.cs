﻿namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.Rest;
    using Discord.WebSocket;

    using GreenPipes;

    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Embeds;
    using NFB.UI.DiscordBot.Events;
    using NFB.UI.DiscordBot.Models;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The update active flight message.
    /// </summary>
    public class UpdateActiveFlightMessageActivity : Activity<FlightState, FlightStartingEvent>, Activity<FlightState, UserJoinedVoiceChannelEvent>, Activity<FlightState, UserLeftVoiceChannelEvent>, Activity<FlightState, UpdatePilotDataInMessage>
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateActiveFlightMessageActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public UpdateActiveFlightMessageActivity(DiscordSocketClient client)
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
        public async Task Execute(BehaviorContext<FlightState, FlightStartingEvent> context, Behavior<FlightState, FlightStartingEvent> next)
        {
            await this.UpdateChannelMessage(
                context.Instance.Origin,
                context.Instance.Destination,
                context.Instance.MessageId,
                context.Instance.VoiceChannelUlongId,
                context.Instance.StartTime,
                context.Instance.VatsimPilotData);
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
            await this.UpdateChannelMessage(
                context.Instance.Origin,
                context.Instance.Destination,
                context.Instance.MessageId,
                context.Instance.VoiceChannelUlongId,
                context.Instance.StartTime,
                context.Instance.VatsimPilotData);
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
            await this.UpdateChannelMessage(
                context.Instance.Origin,
                context.Instance.Destination,
                context.Instance.MessageId,
                context.Instance.VoiceChannelUlongId,
                context.Instance.StartTime,
                context.Instance.VatsimPilotData);
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
        public async Task Execute(BehaviorContext<FlightState, UpdatePilotDataInMessage> context, Behavior<FlightState, UpdatePilotDataInMessage> next)
        {
            await this.UpdateChannelMessage(
                context.Instance.Origin,
                context.Instance.Destination,
                context.Instance.MessageId,
                context.Instance.VoiceChannelUlongId,
                context.Instance.StartTime,
                context.Instance.VatsimPilotData);
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
        public Task Faulted<TException>(BehaviorExceptionContext<FlightState, UpdatePilotDataInMessage, TException> context, Behavior<FlightState, UpdatePilotDataInMessage> next)
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
        /// Update a channel message.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="voiceChannelId">
        /// The voice channel id.
        /// </param>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="vatsimData">
        /// The vatsim data.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task UpdateChannelMessage(
            AirportEntityDto origin,
            AirportEntityDto destination,
            ulong? messageId,
            ulong? voiceChannelId,
            DateTime startTime,
            IList<VatsimPilotData> vatsimData)
        {
            if (messageId == null)
                throw new InvalidOperationException("Message ID is null");

            if (voiceChannelId == null)
                throw new InvalidOperationException("Voice Channel ID is null");

            // Get the channel
            var guild = this.client.Guilds.FirstOrDefault();
            var category = guild?.CategoryChannels.FirstOrDefault(p => p.Name == "flights");

            if (!(category?.Channels.FirstOrDefault(p => p.Name == "flights") is SocketTextChannel textChannel))
                throw new InvalidOperationException($"Unable to find a channel named 'flights'.");

            if (!(category.Channels.FirstOrDefault(p => p.Id == voiceChannelId) is SocketVoiceChannel voiceChannel))
                throw new InvalidOperationException($"Unable to find a channel named 'flights'.");

            // Edit the message
            var embed = await FlightActiveEmbed.CreateEmbed(
                            origin,
                            destination,
                            startTime,
                            voiceChannel,
                            vatsimData);

            if (!(await textChannel.GetMessageAsync((ulong)messageId) is RestUserMessage message))
                throw new InvalidOperationException($"Unable to find a message with ID {messageId}.");

            await message.ModifyAsync(p => p.Embed = embed);
        }

        #endregion Private Methods
    }
}