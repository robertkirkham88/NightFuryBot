namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Automatonymous;

    using GreenPipes;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The remove user from flight activity.
    /// </summary>
    public class RemoveUserFromFlightActivity : Activity<FlightState, UserLeftVoiceChannelEvent>
    {
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
        public async Task Execute(BehaviorContext<FlightState, UserLeftVoiceChannelEvent> context, Behavior<FlightState, UserLeftVoiceChannelEvent> next)
        {
            if (context.Instance.UsersInVoiceChannel.Contains(context.Data.UserId))
                context.Instance.UsersInVoiceChannel.Remove(context.Data.UserId);

            var vatsimData =
                context.Instance.VatsimPilotData.FirstOrDefault(p => p.UserId == context.Data.UserId);

            if (vatsimData != null)
            {
                context.Instance.AvailableColors.Add(vatsimData.AssignedColor); // Re add the color to the available pool.
                context.Instance.VatsimPilotData.Remove(vatsimData);
            }

            await next.Execute(context);
        }

        /// <summary>
        /// The activity faulted.
        /// </summary>
        /// <typeparam name="TException">
        /// The exception.
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
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, UserLeftVoiceChannelEvent, TException> context, Behavior<FlightState, UserLeftVoiceChannelEvent> next)
            where TException : Exception
        {
            await next.Faulted(context);
        }

        /// <summary>
        /// Create scope.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Probe(ProbeContext context)
        {
            context.CreateScope("remove-user-from-flight");
        }

        #endregion Public Methods
    }
}