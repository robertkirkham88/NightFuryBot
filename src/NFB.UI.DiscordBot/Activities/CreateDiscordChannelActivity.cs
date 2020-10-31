namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord;
    using Discord.WebSocket;

    using GreenPipes;

    using MassTransit;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Embeds;
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
        public CreateDiscordChannelActivity(DiscordSocketClient client, IMessageScheduler busScheduler)
        {
            this.client = client;
            this.busScheduler = busScheduler;
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
            // Variables
            var destination = context.Data.Destination;
            var origin = context.Data.Origin;
            var startTime = context.Data.StartTime;

            var announcementChannel = this.client.GetChannel(context.Instance.ChannelData.AnnouncementChannel) as SocketTextChannel;
            var embed = FlightCreatedEmbed.CreateEmbed(origin, destination, startTime);
            if (announcementChannel != null)
            {
                var message = await announcementChannel.SendMessageAsync(embed: embed);
                context.Instance.AnnouncementMessageId = message.Id;
            }

            context.Instance.Destination = context.Data.Destination;
            context.Instance.Origin = context.Data.Origin;
            context.Instance.StartTime = context.Data.StartTime;
            context.Instance.AvailableColors = new List<uint>
                                                   {
                                                       Color.Blue.RawValue,
                                                       Color.Gold.RawValue,
                                                       Color.Magenta.RawValue,
                                                       Color.Teal.RawValue,
                                                       Color.Orange.RawValue,
                                                       Color.Purple.RawValue,
                                                       Color.LightOrange.RawValue,
                                                       Color.LightGrey.RawValue
                                                   };

            if (this.client.GetChannel(context.Instance.ChannelData.BookChannel) is ITextChannel discordChannel)
            {
                var sentMessage = await discordChannel.SendMessageAsync($"Successfully created a new flight from {context.Instance.Origin.ICAO} to {context.Instance.Destination.ICAO} for {context.Instance.StartTime:g}");

                await this.busScheduler.SchedulePublish(
                    DateTime.UtcNow.AddSeconds(30),
                    new DiscordMessageExpiredEvent
                    {
                        ChannelId = sentMessage.Channel.Id,
                        MessageId = sentMessage.Id
                    });
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