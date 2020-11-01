namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.WebSocket;

    using GreenPipes;

    using Microsoft.Extensions.Logging;

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

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<CreateVoiceChannelActivity> logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVoiceChannelActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public CreateVoiceChannelActivity(DiscordSocketClient client, ILogger<CreateVoiceChannelActivity> logger)
        {
            this.client = client;
            this.logger = logger;
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
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            var existingVoiceChannel = this.client.GetChannel(p => p.Name == context.Instance.VoiceChannelName);

            if (existingVoiceChannel == null)
            {
                var voiceChannel = await this.client.CreateVoiceChannelAsync(
                                       context.Instance.ChannelData.Category,
                                       context.Instance.VoiceChannelName);

                context.Instance.VoiceChannelUlongId = voiceChannel.Id;
                context.Instance.VoiceChannelId = voiceChannel.Id.ToGuid();

                this.logger.LogInformation($"SAGA {context.Instance.CorrelationId}: Created new voice channel {context.Instance.VoiceChannelUlongId.GetValueOrDefault()} in {context.Instance.ChannelData.Category}.");
            }
            else
            {
                context.Instance.VoiceChannelUlongId = existingVoiceChannel.Id;
                context.Instance.VoiceChannelId = existingVoiceChannel.Id.ToGuid();

                this.logger.LogInformation($"SAGA {context.Instance.CorrelationId}: Found existing voice channel {context.Instance.VoiceChannelUlongId.GetValueOrDefault()} in {context.Instance.ChannelData.Category}.");
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
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            var existingVoiceChannel = this.client.GetChannel(p => p.Name == context.Instance.VoiceChannelName);

            if (existingVoiceChannel == null)
            {
                var voiceChannel = await this.client.CreateVoiceChannelAsync(
                                       context.Instance.ChannelData.Category,
                                       context.Instance.VoiceChannelName);

                context.Instance.VoiceChannelUlongId = voiceChannel.Id;
                context.Instance.VoiceChannelId = voiceChannel.Id.ToGuid();

                this.logger.LogInformation($"SAGA {context.Instance.CorrelationId}: Created new voice channel {context.Instance.VoiceChannelUlongId.GetValueOrDefault()} in {context.Instance.ChannelData.Category}.");
            }
            else
            {
                context.Instance.VoiceChannelUlongId = existingVoiceChannel.Id;
                context.Instance.VoiceChannelId = existingVoiceChannel.Id.ToGuid();

                this.logger.LogInformation($"SAGA {context.Instance.CorrelationId}: Found existing voice channel {context.Instance.VoiceChannelUlongId.GetValueOrDefault()} in {context.Instance.ChannelData.Category}.");
            }

            await next.Execute(context);
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
            var voiceChannel = this.client.GetChannel(p => p.Name == context.Instance.VoiceChannelName);
            if (voiceChannel != null)
                await this.client.DeleteVoiceChannelAsync(voiceChannel.Id);

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
            var voiceChannel = this.client.GetChannel(p => p.Name == context.Instance.VoiceChannelName);
            if (voiceChannel != null)
                await this.client.DeleteVoiceChannelAsync(voiceChannel.Id);

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
            context.CreateScope("create-discord-voice-channel");
        }

        #endregion Public Methods
    }
}