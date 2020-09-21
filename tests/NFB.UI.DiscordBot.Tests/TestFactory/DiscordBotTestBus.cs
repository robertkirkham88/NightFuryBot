namespace NFB.UI.DiscordBot.Tests.TestFactory
{
    using Autofac;

    using MassTransit;
    using MassTransit.Testing;

    using NFB.UI.DiscordBot.StateMachines;

    /// <summary>
    /// The flight service test bus.
    /// </summary>
    public class DiscordBotTestBus : InMemoryTestHarness
    {
        #region Private Fields

        private readonly IContainer services;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscordBotTestBus"/> class.
        /// </summary>
        /// <param name="builtServices"></param>
        public DiscordBotTestBus(IContainer builtServices)
        {
            this.services = builtServices;
        }

        #endregion Public Constructors

        #region Protected Methods

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(new FlightStateMachine(), this.services);

            base.ConfigureInMemoryReceiveEndpoint(configurator);
        }

        #endregion Protected Methods
    }
}