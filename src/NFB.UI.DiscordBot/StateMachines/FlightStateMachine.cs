namespace NFB.UI.DiscordBot.StateMachines
{
    using System.Linq;

    using Automatonymous;

    using Discord;
    using Discord.WebSocket;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Extensions;
    using NFB.UI.DiscordBot.Persistence;
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
        /// <param name="database">
        /// The database
        /// </param>
        public FlightStateMachine(DiscordSocketClient client, DiscordBotDbContext database)
        {
            // Instance
            this.InstanceState(p => p.CurrentState);

            // Events
            this.Event(() => this.FlightCreatedEvent, x => x.CorrelateById(p => p.Message.Id));
            this.Event(() => this.FlightStartingEvent, x => x.CorrelateById(p => p.Message.Id));
            this.Event(() => this.UserJoinedVoiceChannelEvent, x => x.CorrelateBy(p => p.VoiceChannelId, p => p.Message.ChannelId.ToGuid()));
            this.Event(() => this.UserLeftVoiceChannelEvent, x => x.CorrelateBy(p => p.VoiceChannelId, p => p.Message.ChannelId.ToGuid()));
            this.Event(() => this.VatsimPilotUpdatedEvent, x => x.CorrelateBy((state, context) => state.UsersInVoiceChannel.Contains(context.Message.UserId.ToGuid())));

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
                                    var message = await channel.SendMessageAsync($"{context.Data.Id} -> {context.Data.Origin.ICAO} to {context.Data.Destination.ICAO}: {context.Data.StartTime:s}");

                                    context.Instance.MessageId = message.Id;
                                }
                            })
                    .Then(
                        context =>
                            {
                                context.Instance.Destination = context.Data.Destination.ICAO;
                                context.Instance.Origin = context.Data.Origin.ICAO;
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

                                    context.Instance.VoiceChannelId = voiceChannel.Id.ToGuid();
                                }
                            })
                    .TransitionTo(this.Active));

            this.During(
                this.Active,
                this.When(this.UserJoinedVoiceChannelEvent).Then(context => context.Instance.UsersInVoiceChannel.Add(context.Data.UserId.ToGuid())),
                this.When(this.UserLeftVoiceChannelEvent).Then(context => context.Instance.UsersInVoiceChannel.Remove(context.Data.UserId.ToGuid())),
                this.When(this.VatsimPilotUpdatedEvent).ThenAsync(
                    async (context) =>
                        {
                            var server = client.Guilds.First();
                            var category = server?.CategoryChannels.First(p => p.Name == "flights");

                            if (category?.Channels.First(p => p.Name == "flights") is IMessageChannel channel)
                            {
                                await channel.SendMessageAsync($"{context.Data.VatsimId} -> {context.Data.Longitude} {context.Data.Latitude}");
                            }
                        }));
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

        /// <summary>
        /// Gets or sets the user joined voice channel event.
        /// </summary>
        public Event<UserJoinedVoiceChannelEvent> UserJoinedVoiceChannelEvent { get; set; }

        /// <summary>
        /// Gets or sets the user left voice channel event.
        /// </summary>
        public Event<UserLeftVoiceChannelEvent> UserLeftVoiceChannelEvent { get; set; }

        /// <summary>
        /// Gets or sets the vatsim pilot updated event.
        /// </summary>
        public Event<VatsimPilotUpdatedEvent> VatsimPilotUpdatedEvent { get; set; }

        #endregion Public Properties
    }
}