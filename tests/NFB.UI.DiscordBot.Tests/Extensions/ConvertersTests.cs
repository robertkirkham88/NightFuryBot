namespace NFB.UI.DiscordBot.Tests.Extensions
{
    using System;

    using NFB.UI.DiscordBot.Extensions;

    using Xunit;

    /// <summary>
    /// The converters tests.
    /// </summary>
    public class ConvertersTests
    {
        #region Public Methods

        /// <summary>
        /// The converts to guid.
        /// </summary>
        [Fact]
        public void ConvertsToGuid()
        {
            // Arrange
            var ulongValue = (ulong)365241212057419776;

            // Act
            var result = ulongValue.ToGuid();

            // Assert
            Assert.Equal(result, Guid.Parse("7e040000-98f2-0511-0000-000000000000"));
        }

        /// <summary>
        /// The converts to ulong.
        /// </summary>
        [Fact]
        public void ConvertsToUlong()
        {
            // Arrange
            var guidValue = Guid.Parse("7e040000-98f2-0511-0000-000000000000");

            // Act
            var result = guidValue.ToULong();

            // Assert
            Assert.Equal(result, (ulong)365241212057419776);
        }

        /// <summary>
        /// The uri well formed.
        /// </summary>
        [Fact]
        public void UriWellFormed()
        {
            // Arrange
            var uri = "https://open.mapquestapi.com/staticmap/v5/map?locations=53.365,-2.27089%7Cmarker-start%7C%7C51.4700,-0.4543%7Cmarker-end%7C%7C3.6065,-153.35879%7Cmarker-A&key=z8WHXvhF50CDEdE3GOrleDlWtwOOZVjG";

            // Act
            var isWellFormed = Uri.IsWellFormedUriString(uri, UriKind.Absolute);

            // Assert
            Assert.True(isWellFormed);
        }

        #endregion Public Methods
    }
}