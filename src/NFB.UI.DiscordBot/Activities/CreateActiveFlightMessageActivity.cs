namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.WebSocket;

    using GreenPipes;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Embeds;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The create active flight message activity.
    /// </summary>
    public class CreateActiveFlightMessageActivity : Activity<FlightState, FlightStartingEvent>
    {
        #region Private Fields

        /// <summary>
        /// The client.
        /// </summary>
        private readonly DiscordSocketClient client;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateActiveFlightMessageActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public CreateActiveFlightMessageActivity(DiscordSocketClient client)
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
            if (!(this.client.GetChannel(context.Instance.VoiceChannelUlongId.GetValueOrDefault()) is SocketVoiceChannel voiceChannel))
                throw new InvalidOperationException("Voice Channel cannot be null");

            if (!(this.client.GetChannel(context.Instance.ChannelData.ActiveFlightMessageChannel) is SocketTextChannel activeFlightChannel))
                throw new InvalidOperationException("Active flight channel cannot be null");

            if (!(this.client.GetChannel(context.Instance.ChannelData.AnnouncementChannel) is SocketTextChannel announcementChannel))
                throw new InvalidOperationException("The announcement text channel cannot be null");

            // Delete the old announcement message, the flight is now active.
            if (await announcementChannel.GetMessageAsync(context.Instance.AnnouncementMessageId) is SocketUserMessage announcementMessage)
                await announcementMessage.DeleteAsync();

            // Create a new message.
            var embed = await FlightActiveEmbed.CreateEmbed(
                            context.Instance.Origin,
                            context.Instance.Destination,
                            context.Instance.StartTime,
                            voiceChannel,
                            context.Instance.VatsimPilotData);

            var message = await activeFlightChannel.SendMessageAsync(embed: embed);
            context.Instance.ActiveFlightMessageId = message.Id;

            await next.Execute(context);
        }

        /// <summary>
        /// The message faulted.
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
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, FlightStartingEvent, TException> context, Behavior<FlightState, FlightStartingEvent> next)
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
            context.CreateScope("create-active-flight-message");
        }

        #endregion Public Methods
    }
}