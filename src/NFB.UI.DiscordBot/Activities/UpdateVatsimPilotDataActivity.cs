namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using Discord;

    using GreenPipes;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Models;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The update vatsim pilot data activity.
    /// </summary>
    public class UpdateVatsimPilotDataActivity : Activity<FlightState, VatsimPilotUpdatedEvent>
    {
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
            var exists = context.Instance.VatsimPilotData.Any(p => p.UserId == context.Data.UserId);

            if (!exists)
            {
                // Pilot color.
                var chosenColorUint = context.Instance.AvailableColors.FirstOrDefault();
                if (chosenColorUint == default)
                {
                    chosenColorUint = Color.DarkBlue.RawValue;
                }
                else
                {
                    context.Instance.AvailableColors.Remove(chosenColorUint);
                }

                // New pilot information coming through from vatsim
                context.Instance.VatsimPilotData.Add(new VatsimPilotData
                {
                    DestinationAirport = context.Data.DestinationAirport,
                    Latitude = context.Data.Latitude,
                    Longitude = context.Data.Longitude,
                    OriginAirport = context.Data.OriginAirport,
                    UserId = context.Data.UserId,
                    VatsimId = context.Data.VatsimId,
                    AssignedColor = chosenColorUint
                });
            }
            else
            {
                var pilot = context.Instance.VatsimPilotData.FirstOrDefault(p => p.UserId == context.Data.UserId);

                if (pilot != null)
                {
                    pilot.DestinationAirport = context.Data.DestinationAirport;
                    pilot.Latitude = context.Data.Latitude;
                    pilot.Longitude = context.Data.Longitude;
                    pilot.OriginAirport = context.Data.OriginAirport;
                }
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