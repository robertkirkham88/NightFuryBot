namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Threading.Tasks;

    using AutoMapper;

    using Automatonymous;

    using Discord.WebSocket;

    using GreenPipes;

    using MassTransit;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Embeds;
    using NFB.UI.DiscordBot.Extensions;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The create discord channel activity.
    /// </summary>
    public class CreateDiscordChannelActivity : Activity<FlightState, FlightCreatedEvent>
    {
        #region Private Fields

        /// <summary>
        /// The bus scheduler.
        /// </summary>
        private readonly IMessageScheduler busScheduler;

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDiscordChannelActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="busScheduler">
        /// The bus Scheduler.
        /// </param>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        public CreateDiscordChannelActivity(DiscordSocketClient client, IMessageScheduler busScheduler, IMapper mapper)
        {
            this.client = client;
            this.busScheduler = busScheduler;
            this.mapper = mapper;
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
        public async Task Execute(BehaviorContext<FlightState, FlightCreatedEvent> context, Behavior<FlightState, FlightCreatedEvent> next)
        {
            this.mapper.Map(context.Data, context.Instance);

            var message = await this.client.SendMessageToChannelAsync(
                              context.Instance.ChannelData.AnnouncementChannel,
                              embed: FlightCreatedEmbed.CreateEmbed(context.Instance.Origin, context.Instance.Destination, context.Instance.StartTime));

            context.Instance.AnnouncementMessageId = message.Id;

            var response = await this.client.SendMessageToChannelAsync(
                               context.Instance.ChannelData.BookChannel,
                               $"Successfully created a new flight from {context.Instance.Origin.ICAO} to {context.Instance.Destination.ICAO} for {context.Instance.StartTime:g}");

            await this.busScheduler.SchedulePublish(
                DateTime.UtcNow.AddSeconds(30),
                new DiscordMessageExpiredEvent
                {
                    ChannelId = response.Channel.Id,
                    MessageId = response.Id
                });

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
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, FlightCreatedEvent, TException> context, Behavior<FlightState, FlightCreatedEvent> next)
            where TException : Exception
        {
            // Get the channel
            if (this.client.GetChannel(context.Instance.ChannelData.AnnouncementChannel) is SocketTextChannel announcementChannel)
            {
                var message = await announcementChannel.GetMessageAsync(context.Instance.AnnouncementMessageId);
                if (message != null)
                    await message.DeleteAsync();
            }

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
            context.CreateScope("create-discord-channel");
        }

        #endregion Public Methods
    }
}