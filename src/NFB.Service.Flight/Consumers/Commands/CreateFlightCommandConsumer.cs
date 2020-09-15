namespace NFB.Service.Flight.Consumers.Commands
{
    using System;
    using System.Threading.Tasks;

    using FluentValidation;

    using MassTransit;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Bus.Events;
    using NFB.Domain.Bus.Responses;
    using NFB.Service.Flight.Entities;
    using NFB.Service.Flight.Persistence;
    using NFB.Service.Flight.Repository;
    using NFB.Service.Flight.Validators;

    /// <summary>
    /// The create flight command consumer.
    /// </summary>
    public class CreateFlightCommandConsumer : IConsumer<CreateFlightCommand>
    {
        #region Private Fields

        /// <summary>
        /// The airport repository.
        /// </summary>
        private readonly AirportRepository airportRepository;

        /// <summary>
        /// The database.
        /// </summary>
        private readonly FlightDbContext database;

        /// <summary>
        /// The validator.
        /// </summary>
        private readonly FlightEntityValidator validator;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFlightCommandConsumer"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="airportRepository">
        /// The airport Repository.
        /// </param>
        public CreateFlightCommandConsumer(FlightDbContext database, AirportRepository airportRepository)
        {
            this.database = database;
            this.airportRepository = airportRepository;
            this.validator = new FlightEntityValidator();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Create a new flight.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Consume(ConsumeContext<CreateFlightCommand> context)
        {
            // Transform the message to an entity.
            var entity = new FlightEntity
            {
                Destination = context.Message.Destination,
                Origin = context.Message.Origin,
                StartTime = context.Message.StartTime
            };

            try
            {
                // Validate the entity.
                this.validator.ValidateAndThrow(entity);

                // Add the entity to the database.
                var databaseEntity = await this.database.Flights.AddAsync(entity);
                await this.database.SaveChangesAsync();

                // Airport DTOs
                var originAirport = this.airportRepository.GetAirport(entity.Origin);
                var destinationAirport = this.airportRepository.GetAirport(entity.Destination);

                // Respond to the message.
                await context.RespondAsync(new CreateFlightCommandSuccessResponse { Id = databaseEntity.Entity.Id });
                await context.Publish(new FlightCreatedEvent
                {
                    Destination = new AirportEntityDto { ICAO = destinationAirport.ICAO, Latitude = destinationAirport.ICAO, Longitude = destinationAirport.Longitude, Name = destinationAirport.Name },
                    Origin = new AirportEntityDto { ICAO = originAirport.ICAO, Latitude = originAirport.Latitude, Longitude = originAirport.Longitude, Name = originAirport.Name },
                    StartTime = databaseEntity.Entity.StartTime,
                    Id = databaseEntity.Entity.Id
                });
            }
            catch (Exception ex)
            {
                // Flight addition has failed.
                await context.RespondAsync(new CreateFlightCommandFailResponse { Reason = ex.Message });
            }
        }

        #endregion Public Methods
    }
}