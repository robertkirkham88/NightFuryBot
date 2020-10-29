namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.WebSocket;

    using GreenPipes;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Extensions;
    using NFB.UI.DiscordBot.Services;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The create voice channel activity.
    /// </summary>
    public class CreateVoiceChannelActivity : Activity<FlightState, FlightStartingEvent>, Activity<FlightState, FlightCreatedEvent>
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
        /// Initializes a new instance of the <see cref="CreateVoiceChannelActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="channelService">
        /// The channel Service.
        /// </param>
        public CreateVoiceChannelActivity(DiscordSocketClient client, IChannelService channelService)
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
            // Variables
            var destination = context.Instance.Destination;
            var origin = context.Instance.Origin;

            var channelData = await this.channelService.Get(context.Instance.RequestChannelId);

            // Create the vault channel
            var guild = this.client.Guilds.FirstOrDefault();
            var category = guild?.GetCategoryChannel(channelData.Category);
            var channelName = $"{origin.ICAO}-{destination.ICAO}-{context.Instance.CorrelationId.ToString().Substring(0, 3)}";
            var existingChannel = category?.Channels.FirstOrDefault(p => p.Name == channelName);

            if (existingChannel == null)
            {
                var voiceChannel = await this.client.Guilds.First().CreateVoiceChannelAsync(
                                       channelName,
                                       properties => { properties.CategoryId = category?.Id; });

                context.Instance.VoiceChannelUlongId = voiceChannel.Id;
                context.Instance.VoiceChannelId = voiceChannel.Id.ToGuid();
            }
            else
            {
                context.Instance.VoiceChannelUlongId = existingChannel.Id;
                context.Instance.VoiceChannelId = existingChannel.Id.ToGuid();
            }

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
            // Activity
            var activity = new CreateDiscordChannelActivity(this.client, this.channelService);

            // Execute
            await activity.Execute(context, next);
        }

        /// <summary>
        /// Faulted exception
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
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, FlightStartingEvent, TException> context, Behavior<FlightState, FlightStartingEvent> next)
            where TException : Exception
        {
            if (context.Instance.VoiceChannelUlongId != null)
            {
                // Variables
                var channelId = (ulong)context.Instance.VoiceChannelUlongId;

                // Get the channel
                var guild = this.client.Guilds.FirstOrDefault();
                var category = guild?.CategoryChannels.FirstOrDefault(p => p.Name == "flights");

                if (!(category?.Channels.FirstOrDefault(p => p.Id == channelId) is SocketVoiceChannel channel))
                    throw new InvalidOperationException($"Unable to find a channel named 'flights'.");

                await channel.DeleteAsync();
            }

            await next.Faulted(context);
        }

        /// <summary>
        /// Faulted activity.
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
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, FlightCreatedEvent, TException> context, Behavior<FlightState, FlightCreatedEvent> next)
            where TException : Exception
        {
            // Activity
            var activity = new CreateDiscordChannelActivity(this.client, this.channelService);

            // Faulted
            await activity.Faulted(context, next);
        }

        /// <summary>
        /// Create a scope.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Probe(ProbeContext context)
        {
            context.CreateScope("create-discord-voice-channel");
        }

        #endregion Public Methods
    }
}