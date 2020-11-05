namespace NFB.UI.DiscordBot.Activities
{
    using System;
    using System.Threading.Tasks;

    using Automatonymous;

    using GreenPipes;

    using Microsoft.Extensions.Logging;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The add user to flight activity.
    /// </summary>
    public class AddUserToFlightActivity : Activity<FlightState, UserJoinedVoiceChannelEvent>
    {
        #region Private Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<AddUserToFlightActivity> logger;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddUserToFlightActivity"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AddUserToFlightActivity(ILogger<AddUserToFlightActivity> logger)
        {
            this.logger = logger;
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
        public async Task Execute(BehaviorContext<FlightState, UserJoinedVoiceChannelEvent> context, Behavior<FlightState, UserJoinedVoiceChannelEvent> next)
        {
            this.logger.LogInformation("SAGA {@id}: Received {@data}", context.Instance.CorrelationId, context.Data);

            if (!context.Instance.UsersInVoiceChannel.Contains(context.Data.UserId))
                context.Instance.UsersInVoiceChannel.Add(context.Data.UserId);

            await next.Execute(context);
        }

        /// <summary>
        /// The activity faulted.
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
        public async Task Faulted<TException>(BehaviorExceptionContext<FlightState, UserJoinedVoiceChannelEvent, TException> context, Behavior<FlightState, UserJoinedVoiceChannelEvent> next)
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
            context.CreateScope("add-user-to-flight");
        }

        #endregion Public Methods
    }
}