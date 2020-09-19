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
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The create voice channel activity.
    /// </summary>
    public class CreateVoiceChannelActivity : Activity<FlightState, FlightStartingEvent>, Activity<FlightState, FlightCreatedEvent>
    {
        #region Private Fields

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
        public CreateVoiceChannelActivity(DiscordSocketClient client)
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
            // Variables
            var destination = context.Instance.Destination;
            var origin = context.Instance.Origin;

            // Get the channel
            var guild = this.client.Guilds.FirstOrDefault();
            var category = guild?.CategoryChannels.FirstOrDefault(p => p.Name == "flights");

            if (guild == null || category == null)
                throw new InvalidOperationException("Unable to create a voice channel because was unable to find guild or voice channel");

            // Create the vault channel
            var voiceChannel = await guild.CreateVoiceChannelAsync(
                                   $"{origin.ICAO}-{destination.ICAO}-{context.Instance.CorrelationId.ToString().Substring(0, 3)}",
                                   properties => { properties.CategoryId = category.Id; });

            // Update context data
            context.Instance.VoiceChannelUlongId = voiceChannel.Id;
            context.Instance.VoiceChannelId = voiceChannel.Id.ToGuid();

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
            var activity = new CreateDiscordChannelActivity(this.client);

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
            var activity = new CreateDiscordChannelActivity(this.client);

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