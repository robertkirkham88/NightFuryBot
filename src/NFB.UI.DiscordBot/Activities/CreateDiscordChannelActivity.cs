namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord;
    using Discord.WebSocket;

    using GreenPipes;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Embeds;
    using NFB.UI.DiscordBot.Services;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The create discord channel activity.
    /// </summary>
    public class CreateDiscordChannelActivity : Activity<FlightState, FlightCreatedEvent>
    {
        #region Private Fields

        /// <summary>
        /// The channel service.
        /// </summary>
        private readonly IChannelService channelService;

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
        /// <param name="channelService">
        /// The channel Service.
        /// </param>
        public CreateDiscordChannelActivity(DiscordSocketClient client, IChannelService channelService)
        {
            this.client = client;
            this.channelService = channelService;
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

            var channelData = await this.channelService.Get(context.Instance.RequestChannelId);
            var announcementChannel = this.client.GetChannel(channelData.AnnouncementChannel) as SocketTextChannel;
            var embed = FlightCreatedEmbed.CreateEmbed(origin, destination, startTime);
            if (announcementChannel != null)
            {
                var message = await announcementChannel.SendMessageAsync(embed: embed);

                context.Instance.MessageId = message.Id;
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

            var discordChannel = this.client.GetChannel(context.Instance.RequestChannelId) as ITextChannel;
            if (discordChannel != null)
            {
                await discordChannel.SendMessageAsync(
                    $"Successfully created a new flight from {context.Instance.Origin.ICAO} to {context.Instance.Destination.ICAO} for {context.Instance.StartTime:g}");
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
            if (context.Instance.MessageId != null)
            {
                // Variables
                var messageId = (ulong)context.Instance.MessageId;

                // Get the channel
                var guild = this.client.Guilds.FirstOrDefault();
                var category = guild?.CategoryChannels.FirstOrDefault(p => p.Name == "flights");

                if (!(category?.Channels.FirstOrDefault(p => p.Name == "flights") is SocketTextChannel channel))
                    throw new InvalidOperationException($"Unable to find a channel named 'flights'.");

                var message = await channel.GetMessageAsync(messageId);
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