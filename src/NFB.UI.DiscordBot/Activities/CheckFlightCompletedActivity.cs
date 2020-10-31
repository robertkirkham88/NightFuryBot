namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord.Rest;
    using Discord.WebSocket;

    using GreenPipes;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Schedules;
    using NFB.UI.DiscordBot.Services;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The update voice channel users activity.
    /// </summary>
    public class CheckFlightCompletedActivity : Activity<FlightState, CheckFlightCompletedScheduleMessage>
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
        /// Initializes a new instance of the <see cref="CheckFlightCompletedActivity"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="channelService">
        /// The channel service.
        /// </param>
        public CheckFlightCompletedActivity(DiscordSocketClient client, IChannelService channelService)
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
        public async Task Execute(BehaviorContext<FlightState, CheckFlightCompletedScheduleMessage> context, Behavior<FlightState, CheckFlightCompletedScheduleMessage> next)
        {
            var channelData = await this.channelService.Get(context.Instance.RequestChannelId);
            var category = this.client.Guilds.First().GetCategoryChannel(channelData.Category);
            var voiceChannel = category.Channels.First(p => p.Id == context.Instance.VoiceChannelUlongId) as SocketVoiceChannel;
            var announcementChannel = category.Channels.First(p => p.Id == channelData.AnnouncementChannel) as SocketTextChannel;

            var voiceChannelId = context.Instance.VoiceChannelUlongId;

            if (voiceChannelId != null && voiceChannel != null && voiceChannel.Users.Any())
                await next.Execute(context);

            if (context.Instance.MessageId != null && announcementChannel != null)
            {
                var restMessage = await announcementChannel.GetMessageAsync((ulong)context.Instance.MessageId) as RestUserMessage;
                var socketMessage = await announcementChannel.GetMessageAsync((ulong)context.Instance.MessageId) as SocketUserMessage;
                restMessage?.DeleteAsync();
                socketMessage?.DeleteAsync();
            }

            if (voiceChannel != null)
                await voiceChannel.DeleteAsync();

            await context.Publish(new FlightCompletedEvent { Id = context.Instance.CorrelationId });
            await next.Execute(context);
        }

        /// <summary>
        /// The activity faulted.
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
        public Task Faulted<TException>(BehaviorExceptionContext<FlightState, CheckFlightCompletedScheduleMessage, TException> context, Behavior<FlightState, CheckFlightCompletedScheduleMessage> next)
            where TException : Exception
        {
            return next.Faulted(context);
        }

        /// <summary>
        /// Create a scope.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Probe(ProbeContext context)
        {
            context.CreateScope("check-flight-completed");
        }

        #endregion Public Methods
    }
}