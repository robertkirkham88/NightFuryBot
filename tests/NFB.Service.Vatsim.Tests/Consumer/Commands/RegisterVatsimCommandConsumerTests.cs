namespace NFB.Service.Vatsim.Tests.Consumer.Commands
{
    using System;
    using System.Threading.Tasks;

    using AutoMapper;

    using MassTransit.Testing;

    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.Responses;
    using NFB.Service.Vatsim.Consumers.Commands;
    using NFB.Service.Vatsim.Entities;
    using NFB.Service.Vatsim.Mappings;
    using NFB.Service.Vatsim.Persistence;
    using NFB.Service.Vatsim.Tests.TestFactory;

    using Xunit;

    /// <summary>
    /// The register vatsim command consumer tests.
    /// </summary>
    public class RegisterVatsimCommandConsumerTests
    {
        #region Private Fields

        /// <summary>
        /// The consumer.
        /// </summary>
        private readonly RegisterVatsimCommandConsumer consumer;

        /// <summary>
        /// The database.
        /// </summary>
        private readonly VatsimDbContext database;

        /// <summary>
        /// The harness.
        /// </summary>
        private readonly InMemoryTestHarness harness;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterVatsimCommandConsumerTests"/> class.
        /// </summary>
        public RegisterVatsimCommandConsumerTests()
        {
            var mapperCfg = new MapperConfiguration(cfg => cfg.AddProfile<PilotEntityMapping>());
            var mapper = new Mapper(mapperCfg);

            // Constructors
            this.database = new VatsimDbContext(new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            this.consumer = new RegisterVatsimCommandConsumer(this.database, mapper);
            this.harness = new InMemoryTestHarness();
            this.harness.Consumer(() => this.consumer);
            this.harness.Start().GetAwaiter().GetResult();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The consumer adds new entity.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ConsumerAddsNewEntity()
        {
            // Arrange
            var command = new RegisterVatsimCommand { VatsimId = "SomeRandomId" };
            var mockConsumer = new Mock<ConsumerContextTestObject<RegisterVatsimCommand>>(command);

            // Act
            await this.consumer.Consume(mockConsumer.Object);
            var result = await this.database.Pilots.FirstAsync();

            // Assert
            Assert.Equal(command.VatsimId, result.VatsimId);
        }

        /// <summary>
        /// The consumer consumes message.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ConsumerConsumesMessage()
        {
            // Arrange
            var command = new RegisterVatsimCommand { VatsimId = "SomeRandomId" };

            // Act
            await this.harness.InputQueueSendEndpoint.Send(command);

            // Assert
            Assert.True(await this.harness.Consumed.Any<RegisterVatsimCommand>());
        }

        /// <summary>
        /// The consumer new entity responds correctly.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ConsumerNewEntityRespondsCorrectly()
        {
            // Arrange
            var command = new RegisterVatsimCommand { VatsimId = "SomeRandomId" };
            var mockConsumer = new Mock<ConsumerContextTestObject<RegisterVatsimCommand>>(command);

            // Act
            await this.consumer.Consume(mockConsumer.Object);

            // Assert
            mockConsumer.Verify(m => m.RespondAsync(It.IsAny<RegisterVatsimCommandSuccessResponse>()));
        }

        /// <summary>
        /// The consumer updated entity responds correctly.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ConsumerUpdatedEntityRespondsCorrectly()
        {
            // Arrange
            await this.database.Pilots.AddAsync(new PilotEntity { VatsimId = "SomeRandomId", UserId = "1234" });
            await this.database.SaveChangesAsync();

            var command = new RegisterVatsimCommand { VatsimId = "SomeRandomId2", UserId = "1234" };
            var mockConsumer = new Mock<ConsumerContextTestObject<RegisterVatsimCommand>>(command);

            // Act
            await this.consumer.Consume(mockConsumer.Object);

            // Assert
            mockConsumer.Verify(m => m.RespondAsync(It.IsAny<RegisterVatsimCommandSuccessResponse>()));
        }

        /// <summary>
        /// The consumer updates entity.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task ConsumerUpdatesEntity()
        {
            // Arrange
            await this.database.Pilots.AddAsync(new PilotEntity { VatsimId = "SomeRandomId", UserId = "1234" });
            await this.database.SaveChangesAsync();

            var command = new RegisterVatsimCommand { VatsimId = "SomeRandomId2", UserId = "1234" };
            var mockConsumer = new Mock<ConsumerContextTestObject<RegisterVatsimCommand>>(command);

            // Act
            await this.consumer.Consume(mockConsumer.Object);
            var result = await this.database.Pilots.FirstAsync();

            // Assert
            Assert.Equal(command.VatsimId, result.VatsimId);
        }

        #endregion Public Methods
    }
}