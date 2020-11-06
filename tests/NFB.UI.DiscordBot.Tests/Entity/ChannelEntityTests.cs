namespace NFB.UI.DiscordBot.Tests.Entity
{
    using NFB.UI.DiscordBot.Entities;

    using Xunit;

    /// <summary>
    /// The channel entity tests.
    /// </summary>
    public class ChannelEntityTests
    {
        #region Public Methods

        /// <summary>
        /// The properties initialized correctly.
        /// </summary>
        [Fact]
        public void PropertiesInitializedCorrectly()
        {
            // Arrange
            var entity = new ChannelEntity();

            // Assert
            Assert.Equal(default, entity.AnnouncementChannel);
            Assert.Equal(default, entity.ActiveFlightMessageChannel);
            Assert.Equal(default, entity.BookChannel);
            Assert.Equal(default, entity.Category);
        }

        #endregion Public Methods
    }
}