﻿namespace NFB.UI.DiscordBot.Activities
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
            // Variables
            var voiceChannelId = context.Instance.VoiceChannelUlongId;
            var messageId = context.Instance.MessageId;

            // Get the channel
            var guild = this.client.Guilds.FirstOrDefault();
            var category = guild?.CategoryChannels.FirstOrDefault(p => p.Name == "flights");

            if (guild == null || category == null)
                throw new InvalidOperationException("Unable to find a voice channel because was unable to find guild or voice channel");

            var textChannel = category.Channels.FirstOrDefault(p => p.Name == "flights") as SocketTextChannel;
            if (textChannel == null)
                throw new InvalidOperationException($"Unable to find a channel named 'flights'.");

            var voiceChannel = category.Channels.FirstOrDefault(p => p.Id == voiceChannelId) as SocketVoiceChannel;
            if (voiceChannel == null)
                throw new InvalidOperationException($"Unable to find a voice channel.");

            var users = voiceChannel.Users;

            if (!users.Any())
            {
                if (messageId != null)
                {
                    var restMessage = await textChannel.GetMessageAsync((ulong)messageId) as RestUserMessage;
                    var socketMessage = await textChannel.GetMessageAsync((ulong)messageId) as SocketUserMessage;
                    restMessage?.DeleteAsync();
                    socketMessage?.DeleteAsync();
                }

                if (voiceChannelId != null)
                {
                    await voiceChannel?.DeleteAsync();
                }

                await context.Publish(new FlightCompletedEvent { Id = context.Instance.CorrelationId });
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