namespace NFB.UI.DiscordBot.Commands.Flight
{
    using System;
    using System.Threading.Tasks;

    using Discord.Commands;

    using MassTransit;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.Responses;

    /// <summary>
    /// Add a new flight.
    /// </summary>
    public class AddFlightCommand : BaseFlightCommand
    {
        #region Private Fields

        /// <summary>
        /// The request.
        /// </summary>
        private readonly IRequestClient<CreateFlightCommand> request;

        #endregion Private Fields

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFlightCommand"/> class.
        /// </summary>
        /// <param name="bus">
        /// The bus.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        protected AddFlightCommand(IBus bus, IRequestClient<CreateFlightCommand> request)
            : base(bus)
        {
            this.request = request;
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
        public async Task<RuntimeResult> ExecuteAsync(string origin, string destination, [Remainder] DateTime startTime)
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

            try
            {
                var (successful, failed) = await this.request.GetResponse<CreateFlightCommandSuccessResponse, CreateFlightCommandFailResponse>(
                                               new CreateFlightCommand
                                               {
                                                   Destination = destination,
                                                   Origin = origin,
                                                   StartTime = startTime
                                               });

                if (successful.IsCompletedSuccessfully)
                {
                    await successful;
                    return CommandResult.FromSuccess($"Adding a flight from {origin} to {destination} for {startTime.ToUniversalTime():s}.");
                }

                await failed;
                return CommandResult.FromError($"Failed to add a flight from {origin} to {destination} for {startTime.ToUniversalTime():s}.");
            }
            catch (Exception ex)
            {
                return CommandResult.FromError(ex.Message);
            }
        }

        #endregion Public Methods
    }
}