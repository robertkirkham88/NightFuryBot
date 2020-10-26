namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Automatonymous;

    using Discord;

    using GreenPipes;

    using Microsoft.Extensions.Logging;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Models;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The update vatsim pilot data activity.
    /// </summary>
    public class UpdateVatsimPilotDataActivity : Activity<FlightState, VatsimPilotUpdatedEvent>
    {
        #region Private Fields

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        private readonly ILogger<UpdateVatsimPilotDataActivity> logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateVatsimPilotDataActivity"/> class.
        /// </summary>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UpdateVatsimPilotDataActivity(IMapper mapper, ILogger<UpdateVatsimPilotDataActivity> logger)
        {
            this.mapper = mapper;
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The accept.
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
        public async Task Execute(BehaviorContext<FlightState, VatsimPilotUpdatedEvent> context, Behavior<FlightState, VatsimPilotUpdatedEvent> next)
        {
            this.logger.LogInformation("Updating VatsimPilotData");
            var pilot = context.Instance.VatsimPilotData.FirstOrDefault(p => p.UserId == context.Data.UserId);

            if (pilot == null)
            {
                var chosenColorUint = context.Instance.AvailableColors.FirstOrDefault();
                if (chosenColorUint == default)
                    chosenColorUint = Color.DarkBlue.RawValue;
                else
                    context.Instance.AvailableColors.Remove(chosenColorUint);

                context.Instance.VatsimPilotData.Add(this.mapper.Map(context.Data, new VatsimPilotModel { AssignedColor = chosenColorUint }));
            }
            else
            {
                this.mapper.Map(context.Data, pilot);
            }

            await next.Execute(context);
        }

        /// <summary>
        /// The update faulted.
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
        public Task Faulted<TException>(BehaviorExceptionContext<FlightState, VatsimPilotUpdatedEvent, TException> context, Behavior<FlightState, VatsimPilotUpdatedEvent> next)
            where TException : Exception
        {
            var pilot = context.Instance.VatsimPilotData.FirstOrDefault(p => p.UserId == context.Data.UserId);

            if (pilot == null) return next.Faulted(context);

            if (pilot.AssignedColor != Color.DarkBlue.RawValue)
                context.Instance.AvailableColors.Add(pilot.AssignedColor);

            context.Instance.VatsimPilotData.Remove(pilot);

            return next.Faulted(context);
        }

        /// <summary>
        /// The probe.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Probe(ProbeContext context)
        {
            context.CreateScope("update-vatsim-pilot");
        }

        #endregion Public Methods
    }
}