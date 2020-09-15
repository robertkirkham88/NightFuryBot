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

        #endregion Public Methods
    }
}