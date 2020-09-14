namespace NFB.Service.Flight.Tests.TestFactory
{
    using System;

    using MassTransit.Testing;

    using Microsoft.EntityFrameworkCore;

    using NFB.Service.Flight.Persistence;

    /// <summary>
    /// The base consumer test object.
    /// </summary>
    /// <typeparam name="TConsumer">
    /// The consumer that we are testing.
    /// </typeparam>
    public abstract class ConsumerTestObject<TConsumer>
    {
        #region Protected Fields

        /// <summary>
        /// The consumer.
        /// </summary>
        protected TConsumer consumer;

        /// <summary>
        /// The database.
        /// </summary>
        protected FlightDbContext database;

        /// <summary>
        /// The harness.
        /// </summary>
        protected InMemoryTestHarness harness;

        #endregion Protected Fields

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerTestObject{TConsumer}"/> class.
        /// </summary>
        protected ConsumerTestObject()
        {
            this.database = new FlightDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            this.consumer = (TConsumer)Activator.CreateInstance(typeof(TConsumer), this.database);

            this.harness = new InMemoryTestHarness();
        }

        #endregion Protected Constructors
    }
}