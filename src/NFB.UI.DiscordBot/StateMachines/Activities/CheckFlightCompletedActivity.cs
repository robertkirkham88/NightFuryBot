﻿namespace NFB.UI.DiscordBot.StateMachines.Activities
{
    using System;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.WebSocket;

    using GreenPipes;

    using Microsoft.Extensions.Logging;

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
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            if (this.client.GetChannel(context.Instance.VoiceChannelId) is SocketVoiceChannel
                    channel && channel.Users.Count == 0)
            {
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