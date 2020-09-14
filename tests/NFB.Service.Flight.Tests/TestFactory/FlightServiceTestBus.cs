namespace NFB.Service.Flight.Tests.TestFactory
{
    using MassTransit;
    using MassTransit.Testing;

    /// <summary>
    /// The flight service test bus.
    /// </summary>
    public class FlightServiceTestBus : InMemoryTestHarness
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
            configurator.UseInMemoryScheduler("service_flight_memory");

            base.ConfigureInMemoryBus(configurator);
        }

        #endregion Protected Methods
    }
}