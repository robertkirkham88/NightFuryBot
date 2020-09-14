namespace NFB.UI.Api.Controllers
{
    using System;
    using System.Threading.Tasks;

    using FluentValidation;

    using MassTransit;

    using Microsoft.AspNetCore.Mvc;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.Responses;
    using NFB.Domain.Bus.Validators;

    /// <summary>
    /// The flight controller.
    /// </summary>
    [ApiController]
    [Route("Flight")]
    public class FlightController : Controller
    {
        #region Private Fields

        /// <summary>
        /// The create flight request.
        /// </summary>
        private readonly IRequestClient<CreateFlightCommand> createFlightRequest;

        /// <summary>
        /// The request.
        /// </summary>
        private readonly IRequestClient<GetFlightsCommand> getFlightsRequest;

        /// <summary>
        /// The validator.
        /// </summary>
        private readonly CreateFlightCommandValidator validator;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightController"/> class.
        /// </summary>
        /// <param name="getFlightsRequest">
        /// The request.
        /// </param>
        /// <param name="createFlightRequest">
        /// The create Flight Request.
        /// </param>
        public FlightController(IRequestClient<GetFlightsCommand> getFlightsRequest, IRequestClient<CreateFlightCommand> createFlightRequest)
        {
            this.getFlightsRequest = getFlightsRequest;
            this.createFlightRequest = createFlightRequest;
            this.validator = new CreateFlightCommandValidator();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Add a new flight.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Add(string origin, string destination, DateTime startTime)
        {
            var command = new CreateFlightCommand
            {
                Destination = destination,
                Origin = origin,
                StartTime = startTime
            };

            try
            {
                this.validator.ValidateAndThrow(command);

                var (completed, failed) =
                    await this.createFlightRequest.GetResponse<CreateFlightCommandSuccessResponse, CreateFlightCommandFailResponse>(command);

                if (completed.IsCompletedSuccessfully)
                {
                    var response = await completed;

                    return this.Ok(response.Message);
                }
                else
                {
                    var response = await failed;

                    return this.BadRequest(response.Message.Reason);
                }
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all flights.
        /// </summary>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await this.getFlightsRequest.GetResponse<GetFlightsCommandResponse>(new GetFlightsCommand());
            return this.Ok(response.Message.Flights);
        }

        #endregion Public Methods
    }
}