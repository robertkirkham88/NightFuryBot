namespace NFB.Service.Flight.Activities
{
    using System;
    using System.Threading.Tasks;

    using Automatonymous;

    using GreenPipes;

    using NFB.Domain.Bus.DTOs;
    using NFB.Domain.Bus.Events;
    using NFB.Service.Flight.Repositories;
    using NFB.Service.Flight.States;

    /// <summary>
    /// The create flight activity.
    /// </summary>
    public class CreateFlightActivity : Activity<FlightState, FlightSubmittedEvent>
    {
        #region Private Fields

        /// <summary>
        /// The airport repository.
        /// </summary>
        private readonly AirportRepository airportRepository;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFlightActivity"/> class.
        /// </summary>
        /// <param name="airportRepository">
        /// The airport Repository.
        /// </param>
        public CreateFlightActivity(AirportRepository airportRepository)
        {
            this.airportRepository = airportRepository;
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
        /// Execute the context.
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
        public async Task Execute(BehaviorContext<FlightState, FlightSubmittedEvent> context, Behavior<FlightState, FlightSubmittedEvent> next)
        {
            var originAirport = this.airportRepository.GetAirport(context.Data.Origin);
            var destinationAirport = this.airportRepository.GetAirport(context.Data.Destination);

            context.Instance.Destination = destinationAirport;
            context.Instance.Origin = originAirport;
            context.Instance.StartTime = context.Data.StartTime;

            if (context.Instance.Destination == null)
                throw new InvalidOperationException($"The airport destination cannot be found ({context.Data.Destination})");

            if (context.Instance.Origin == null)
                throw new InvalidOperationException($"The origin airport cannot be found ({context.Data.Origin})");

            if (context.Instance.StartTime < DateTime.UtcNow.AddSeconds(15))
                throw new InvalidOperationException($"The start time must be at least 15 seconds in the future");

            await context.Publish(new FlightCreatedEvent
            {
                Destination = new AirportEntityDto { ICAO = destinationAirport.ICAO, Latitude = destinationAirport.Latitude, Longitude = destinationAirport.Longitude, Name = destinationAirport.Name },
                Origin = new AirportEntityDto { ICAO = originAirport.ICAO, Latitude = originAirport.Latitude, Longitude = originAirport.Longitude, Name = originAirport.Name },
                StartTime = context.Data.StartTime,
                Id = context.Data.Id
            });

            await next.Execute(context);
        }

        /// <summary>
        /// The activity has faulted.
        /// </summary>
        /// <typeparam name="TException">
        /// The exception type.
        /// </typeparam>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="next">
        /// The next.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, FlightSubmittedEvent, TException> context, Behavior<FlightState, FlightSubmittedEvent> next)
                    where TException : Exception
        {
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
            context.CreateScope("create-flight-activity");
        }

        #endregion Public Methods
    }
}