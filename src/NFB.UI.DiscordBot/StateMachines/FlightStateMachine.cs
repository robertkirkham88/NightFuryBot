namespace NFB.UI.DiscordBot.StateMachines
{
    using System.Linq;

    using Automatonymous;

    using Discord;
    using Discord.WebSocket;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The flight state machine.
    /// </summary>
    public class FlightStateMachine : MassTransitStateMachine<FlightState>
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightStateMachine"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public FlightStateMachine(DiscordSocketClient client)
        {
            // Instance
            this.InstanceState(p => p.CurrentState);

            // Events
            this.Event(() => this.FlightCreatedEvent, x => x.CorrelateById(p => p.Message.Id));
            this.Event(() => this.FlightStartingEvent, x => x.CorrelateById(p => p.Message.Id));

            // Work flow
            this.Initially(
                this.When(this.FlightCreatedEvent)
                    .ThenAsync(
                        async (context) =>
                            {
                                // Post a message to the server.
                                var server = client.Guilds.First();
                                var category = server?.CategoryChannels.First(p => p.Name == "flights");

                                if (category?.Channels.First(p => p.Name == "flights") is IMessageChannel channel)
                                {
                                    var message = await channel.SendMessageAsync($"{context.Data.Id} -> {context.Data.Origin} to {context.Data.Destination}: {context.Data.StartTime:s}");

                                    context.Instance.MessageId = message.Id;
                                }
                            })
                .TransitionTo(this.Created));

            this.During(
                this.Created,
                this.When(this.FlightStartingEvent)
                    .ThenAsync(
                        async (context) =>
                            {
                                var server = client?.Guilds.First();
                                var category = server?.CategoryChannels.First(p => p.Name == "flights");

                                if (server != null && category != null)
                                {
                                    var voiceChannel = await server.CreateVoiceChannelAsync(
                                        $"{context.Data.Origin}-{context.Data.Destination}-{context.Data.Id.ToString().Substring(0, 3)}",
                                        f =>
                                            {
                                                f.CategoryId = category.Id;
                                            });

                                    context.Instance.VoiceChannelId = voiceChannel.Id;
                                }
                            })
                    .TransitionTo(this.Active));

            this.During(this.Active);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the active.
        /// </summary>
        public State Active { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public State Created { get; set; }

        /// <summary>
        /// Gets or sets the flight created event.
        /// </summary>
        public Event<FlightCreatedEvent> FlightCreatedEvent { get; set; }

        /// <summary>
        /// Gets or sets the flight starting event.
        /// </summary>
        public Event<FlightStartingEvent> FlightStartingEvent { get; set; }

        #endregion Public Properties
    }
}