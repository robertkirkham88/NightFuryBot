namespace NFB.UI.DiscordBot.StateMachines.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.WebSocket;

    using GreenPipes;

    using MassTransit;

    using Microsoft.Extensions.Logging;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Schedules;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The update voice channel users activity.
    /// </summary>
    public class UpdateVoiceChannelUsersActivity : Activity<FlightState, UpdateVoiceChannelUsersScheduleMessage>
    {
        #region Private Fields

        /// <summary>
        /// The bus.
        /// </summary>
        private readonly IBus bus;

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UpdateVoiceChannelUsersActivity> logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateVoiceChannelUsersActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="bus">
        /// The bus.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UpdateVoiceChannelUsersActivity(DiscordSocketClient client, IBus bus, ILogger<UpdateVoiceChannelUsersActivity> logger)
        {
            this.client = client;
            this.bus = bus;
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The accept.
        /// </summary>
        /// <param name="visitor">
        /// The visitor.
        /// </param>
        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// The execute the context.
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
        public async Task Execute(BehaviorContext<FlightState, UpdateVoiceChannelUsersScheduleMessage> context, Behavior<FlightState, UpdateVoiceChannelUsersScheduleMessage> next)
        {
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            if (this.client.GetChannel(context.Instance.VoiceChannelId) is SocketVoiceChannel channel)
            {
                foreach (var user in channel.Users.ToList().Where(user => !context.Instance.UsersInVoiceChannel.Contains(user.Id)))
                {
                    await this.bus.Publish(
                        new UserJoinedVoiceChannelEvent { ChannelId = channel.Id, UserId = user.Id });
                }

                foreach (var userId in context.Instance.UsersInVoiceChannel)
                {
                    if (channel.Users.FirstOrDefault(p => p.Id == userId) == null)
                    {
                        await this.bus.Publish(
                            new UserLeftVoiceChannelEvent { ChannelId = channel.Id, UserId = userId });
                    }
                }
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
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, UpdateVoiceChannelUsersScheduleMessage, TException> context, Behavior<FlightState, UpdateVoiceChannelUsersScheduleMessage> next)
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
            context.CreateScope("update-voice-channel-users");
        }

        #endregion Public Methods
    }
}