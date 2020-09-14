namespace NFB.UI.DiscordBot.Tests.TestFactory
{
    using MassTransit;
    using MassTransit.Testing;

    /// <summary>
    /// The flight service test bus.
    /// </summary>
    public class DiscordBotTestBus : InMemoryTestHarness
    {
        #region Protected Methods

        /// <summary>
        /// Add a scheduler.
        /// </summary>
        /// <param name="configurator">
        /// The configurator.
        /// </param>
        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);
        }

        #endregion Protected Methods
    }
}