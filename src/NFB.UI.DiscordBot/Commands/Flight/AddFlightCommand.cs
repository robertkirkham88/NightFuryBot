namespace NFB.UI.DiscordBot.Commands.Flight
{
    using System;
    using System.Threading.Tasks;

    using Discord;
    using Discord.Commands;

    using MassTransit;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Services;

    /// <summary>
    /// Add a new flight.
    /// </summary>
    public class AddFlightCommand : BaseFlightCommand
    {
        #region Private Fields

        /// <summary>
        /// The channel service.
        /// </summary>
        private readonly IChannelService channelService;

        #endregion Private Fields

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFlightCommand"/> class.
        /// </summary>
        /// <param name="bus">
        /// The bus.
        /// </param>
        /// <param name="channelService">
        /// The channel service.
        /// </param>
        protected AddFlightCommand(IBus bus, IChannelService channelService)
            : base(bus)
        {
            this.channelService = channelService;
        }

        #endregion Protected Constructors

        #region Public Methods

        /// <summary>
        /// Add a new scheduled flight.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="startTime">
        /// The scheduled time.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Name("Add Flight")]
        [Command("Add")]
        [Alias("a")]
        [Summary("!flight add [OriginICAO] [DestinationICAO] [StartTimeUTC]\r\nAdd a new flight, times are specified in UTC format and must be at least 15 minutes in the future.\r\nExamples:\r\n!flight add EGCC EGLL 23:30\r\n!flight add EGCC EGLL 09/19/2020 23:30\r\n!flight add EGCC EGLL 09/19/2020 23:30:15")]
        public async Task ExecuteAsync(string origin, string destination, [Remainder] DateTime startTime)
        {
            /*
             * User will type the command !flight add with the origin destination and scheduled time specified.
             * For example:
             * !flight add EIDW EGCC 12:30
             * !flight add EIDW EGCC 2020-08-19 12:30
             *
             * All times will be in Zulu time/converted to Zulu time and checked to ensure it is in the future.
             * If it is not in the future, it should display an error message.
             */

            var channel = await this.channelService.Get(this.Context.Message.Channel.Id);

            if (channel == null)
            {
                await this.Context.Channel.SendMessageAsync(
                    "It looks like you cannot book a flight from here! Please try again from one of the booking channels.");

                return;
            }

            var message = new FlightSubmittedEvent
            {
                Id = Guid.NewGuid(),
                Destination = destination,
                Origin = origin,
                StartTime = startTime,
                RequestMessageId = this.Context.Message.Id,
                RequestCategoryId = channel.Category,
                RequestChannelId = channel.BookChannel
            };

            await this.Bus.Publish(message);
        }

        #endregion Public Methods
    }
}