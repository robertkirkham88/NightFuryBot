namespace NFB.UI.DiscordBot.Tests.Mappings
{
    using AutoMapper;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Mappings;
    using NFB.UI.DiscordBot.Models;

    using Xunit;

    /// <summary>
    /// The vatsim pilot updated mapping tests.
    /// </summary>
    public class VatsimPilotUpdatedMappingTests
    {
        #region Private Fields

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VatsimPilotUpdatedMappingTests"/> class.
        /// </summary>
        public VatsimPilotUpdatedMappingTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<VatsimPilotUpdatedMapping>());
            this.mapper = config.CreateMapper();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The mapper with constructor keeps color.
        /// </summary>
        [Fact]
        public void MapperWithConstructorKeepsColor()
        {
            var origin = "EGKK";
            var destination = "LEAL";
            var latitude = 3.5;
            var longitude = 4.5;

            // Arrange
            var pilotUpdatedEvent = new VatsimPilotUpdatedEvent
            {
                PlannedDestinationAirport = destination,
                PlannedDepartureAirport = origin,
                Latitude = latitude,
                Longitude = longitude,
                UserId = 12354571241411,
                Cid = "12380917241"
            };

            // Act
            var result = this.mapper.Map(pilotUpdatedEvent, new VatsimPilotModel { AssignedColor = 1238175 });

            // Assert
            Assert.Equal((uint)1238175, result.AssignedColor);
        }

        /// <summary>
        /// The mapping does not override existing color.
        /// </summary>
        [Fact]
        public void MappingDoesNotOverrideExistingColor()
        {
            // Arrange
            var pilotUpdatedEvent = new VatsimPilotUpdatedEvent
            {
                PlannedDestinationAirport = "EGCC",
                PlannedDepartureAirport = "EGLL",
                Latitude = 50.50,
                Longitude = 40.40,
                UserId = 12354571241411,
                Cid = "12380917241"
            };
            var pilotData = new VatsimPilotModel
            {
                PlannedDestinationAirport = "EGCC",
                PlannedDepartureAirport = "EGLL",
                Latitude = 50.50,
                Longitude = 40.40,
                UserId = 12354571241411,
                VatsimId = "12380917241",
                AssignedColor = 1238175
            };

            // Act
            this.mapper.Map(pilotUpdatedEvent, pilotData);

            // Assert
            Assert.Equal((uint)1238175, pilotData.AssignedColor);
        }

        /// <summary>
        /// The mapping maps correctly.
        /// </summary>
        [Fact]
        public void MappingMapsCorrectly()
        {
            var origin = "EGKK";
            var destination = "LEAL";
            var latitude = 3.5;
            var longitude = 4.5;

            // Arrange
            var pilotUpdatedEvent = new VatsimPilotUpdatedEvent
            {
                PlannedDestinationAirport = destination,
                PlannedDepartureAirport = origin,
                Latitude = latitude,
                Longitude = longitude,
                UserId = 12354571241411,
                Cid = "12380917241"
            };
            var pilotData = new VatsimPilotModel
            {
                PlannedDestinationAirport = "EGCC",
                PlannedDepartureAirport = "EGLL",
                Latitude = 50.50,
                Longitude = 40.40,
                UserId = 12354571241411,
                VatsimId = "12380917241",
                AssignedColor = 1238175
            };

            // Act
            this.mapper.Map(pilotUpdatedEvent, pilotData);

            // Assert
            Assert.Equal(pilotData.PlannedDestinationAirport, origin);
            Assert.Equal(pilotData.PlannedDepartureAirport, destination);
            Assert.Equal(pilotData.Latitude, latitude);
            Assert.Equal(pilotData.Longitude, longitude);
        }

        #endregion Public Methods
    }
}