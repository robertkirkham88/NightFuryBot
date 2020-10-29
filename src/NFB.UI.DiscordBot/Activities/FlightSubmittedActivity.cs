namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Threading.Tasks;

    using Automatonymous;

    using GreenPipes;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The flight submitted activity.
    /// </summary>
    public class FlightSubmittedActivity : Activity<FlightState, FlightSubmittedEvent>
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
            // Pull through instance data
            context.Instance.RequestMessageId = context.Data.RequestMessageId;
            context.Instance.RequestCategoryId = context.Data.RequestCategoryId;
            context.Instance.RequestChannelId = context.Data.RequestChannelId;

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
            context.CreateScope("flight-submitted-activity");
        }

        #endregion Public Methods
    }
}