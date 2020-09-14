namespace NFB.Service.Flight.Consumers.Commands
{
    using System.Linq;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.EntityFrameworkCore;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Bus.Responses;
    using NFB.Service.Flight.Persistence;

    /// <summary>
    /// The get flights command consumer.
    /// </summary>
    public class GetFlightsCommandConsumer : IConsumer<GetFlightsCommand>
    {
        #region Private Fields

        /// <summary>
        /// The database.
        /// </summary>
        private readonly FlightDbContext database;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFlightsCommandConsumer"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        public GetFlightsCommandConsumer(FlightDbContext database)
        {
            this.database = database;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Return a list of flights.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Consume(ConsumeContext<GetFlightsCommand> context)
        {
            var entities = await this.database.Flights.ToListAsync();

            await context.RespondAsync(
                new GetFlightsCommandResponse
                {
                    Flights = entities.Select(p => new FlightEntityDto
                    {
                        Id = p.Id,
                        Destination = p.Destination,
                        Origin = p.Origin,
                        StartTime = p.StartTime
                    })
                });
        }

        #endregion Public Methods
    }
}