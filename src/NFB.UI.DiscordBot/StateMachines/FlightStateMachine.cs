namespace NFB.UI.DiscordBot.StateMachines
{
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord;
    using Discord.Rest;
    using Discord.WebSocket;

    using Microsoft.Extensions.Logging;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Embeds;
    using NFB.UI.DiscordBot.Extensions;
    using NFB.UI.DiscordBot.Persistence;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The flight state machine.
    /// </summary>
    public class FlightStateMachine : MassTransitStateMachine<FlightState>
    {
        #region Private Fields

        /// <summary>
        /// The discord socket client.
        /// </summary>
        private readonly DiscordSocketClient client;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<FlightStateMachine> logger;

        #endregion Private Fields

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
        /// <param name="logger">
        /// The logger.
        /// </param>
        public FlightStateMachine(DiscordSocketClient client, DiscordBotDbContext database, ILogger<FlightStateMachine> logger)
        {
            this.client = client;
            this.logger = logger;

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
                                var channel = this.GetChannel("Flights", "flights");
                                if (channel != null && !context.Instance.MessageId.HasValue)
                                {
                                    var message = await channel.SendMessageAsync(
                                                      string.Empty,
                                                      embed: FlightCreatedEmbed.CreateEmbed(
                                                          context.Data.Origin,
                                                          context.Data.Destination,
                                                          context.Data.StartTime));

                                    context.Instance.MessageId = message.Id;
                                }
                            })
                    .Then(
                        context =>
                            {
                                context.Instance.Destination = context.Data.Destination;
                                context.Instance.Origin = context.Data.Origin;
                                context.Instance.StartTime = context.Data.StartTime;
                            })
                .TransitionTo(this.Created));

            this.During(
                this.Created,
                this.When(this.FlightStartingEvent)
                    .ThenAsync(
                        async (context) =>
                            {
                                var guild = this.GetSocketGuildServer();
                                var category = this.GetCategory("Flights");
                                RestVoiceChannel voiceChannel = null;
                                if (category != null)
                                {
                                    voiceChannel = await guild.CreateVoiceChannelAsync(
                                                           $"{context.Data.Origin}-{context.Data.Destination}-{context.Data.Id.ToString().Substring(0, 3)}",
                                                           f => { f.CategoryId = category.Id; });
                                    context.Instance.VoiceChannelId = voiceChannel.Id.ToGuid();
                                    context.Instance.VoiceChannelUlongId = voiceChannel.Id;
                                }

                                if (context.Instance.MessageId != null)
                                {
                                    var message = await this.GetMessage("Flights", "flights", (ulong)context.Instance.MessageId);

                                    if (message != null)
                                    {
                                        var socketMessage = message;
                                        var embed = await FlightActiveEmbed.CreateEmbed(
                                                        context.Instance.Origin,
                                                        context.Instance.Destination,
                                                        context.Data.StartTime,
                                                        voiceChannel);

                                        await socketMessage.ModifyAsync(p => p.Embed = embed);
                                    }
                                }
                            })
                    .TransitionTo(this.Active));

            this.During(
                this.Active,
                this.When(this.UserJoinedVoiceChannelEvent)
                    .Then(context => context.Instance.UsersInVoiceChannel.Add(context.Data.UserId.ToGuid()))
                    .ThenAsync(async (context) =>
                        {
                            if (context.Instance.MessageId != null)
                            {
                                var message = await this.GetMessage("Flights", "flights", (ulong)context.Instance.MessageId);
                                var voiceChannel = this.GetVoiceChannel("Flights", context.Instance.VoiceChannelUlongId);

                                if (message != null)
                                {
                                    var socketMessage = message;
                                    var embed = await FlightActiveEmbed.CreateEmbed(
                                                    context.Instance.Origin,
                                                    context.Instance.Destination,
                                                    context.Instance.StartTime,
                                                    (IGuildChannel)voiceChannel);

                                    await socketMessage.ModifyAsync(p => p.Embed = embed);
                                }
                            }
                        }),
                this.When(this.UserLeftVoiceChannelEvent)
                    .Then(context => context.Instance.UsersInVoiceChannel.Remove(context.Data.UserId.ToGuid()))
                    .ThenAsync(async (context) =>
                        {
                            if (context.Instance.MessageId != null)
                            {
                                var message = await this.GetMessage("Flights", "flights", (ulong)context.Instance.MessageId);
                                var voiceChannel = this.GetVoiceChannel("Flights", context.Instance.VoiceChannelUlongId);

                                if (message != null)
                                {
                                    var socketMessage = message;
                                    var embed = await FlightActiveEmbed.CreateEmbed(
                                                    context.Instance.Origin,
                                                    context.Instance.Destination,
                                                    context.Instance.StartTime,
                                                    (IGuildChannel)voiceChannel);

                                    await socketMessage.ModifyAsync(p => p.Embed = embed);
                                }
                            }
                        }),
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

        #region Private Methods

        /// <summary>
        /// Get a category.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private SocketCategoryChannel GetCategory(string name)
        {
            var guild = this.GetSocketGuildServer();

            var category = guild?.CategoryChannels.FirstOrDefault(p => p.Name == "flights");

            if (category == null)
                this.logger.LogDebug($"No category found named {name}");

            return category;
        }

        /// <summary>
        /// Get a channel from a category
        /// </summary>
        /// <param name="categoryName">
        /// The category name.
        /// </param>
        /// <param name="channelName">
        /// The channel name.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private ISocketMessageChannel GetChannel(string categoryName, string channelName)
        {
            var category = this.GetCategory(categoryName);

            if (category == null)
            {
                this.logger.LogDebug("Unable to find category for message");
                return null;
            }

            var channel = category.Channels.FirstOrDefault(p => p.Name == channelName);

            if (channel == null)
            {
                this.logger.LogDebug($"No channel named {channelName} found in {category.Name} category");
            }

            return (ISocketMessageChannel)channel;
        }

        /// <summary>
        /// Get a message from a specific channel.
        /// </summary>
        /// <param name="categoryName">
        /// The category name.
        /// </param>
        /// <param name="channelName">
        /// The channel name.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<RestUserMessage> GetMessage(string categoryName, string channelName, ulong messageId)
        {
            var channel = this.GetChannel(categoryName, channelName);

            if (channel == null)
            {
                this.logger.LogDebug("Unable to find a message because the channel doesn't exist");
                return null;
            }

            var message = await channel.GetMessageAsync(messageId);

            if (message == null)
            {
                this.logger.LogDebug($"No message with {messageId} found in {channelName} channel.");
            }

            return message as RestUserMessage;
        }

        /// <summary>
        /// Get the socket guild server.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private SocketGuild GetSocketGuildServer()
        {
            var guild = this.client.Guilds.FirstOrDefault();

            if (guild == null)
                this.logger.LogDebug("No guild has been found.");

            return guild;
        }

        /// <summary>
        /// The get voice channel.
        /// </summary>
        /// <param name="categoryName">
        /// The category Name.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ISocketAudioChannel"/>.
        /// </returns>
        private ISocketAudioChannel GetVoiceChannel(string categoryName, ulong id)
        {
            var category = this.GetCategory(categoryName);

            if (category == null)
            {
                this.logger.LogDebug("Unable to find category for message");
                return null;
            }

            var channel = category.Channels.FirstOrDefault(p => p.Id == id);

            if (channel == null)
            {
                this.logger.LogDebug($"No voice channel with {id} found in {category.Name} category");
            }

            return (ISocketAudioChannel)channel;
        }

        #endregion Private Methods
    }
}