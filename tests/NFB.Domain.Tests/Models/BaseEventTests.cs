namespace NFB.Domain.Tests.Models
{
    using System;

    using NFB.Domain.Models;

    using Xunit;

    /// <summary>
    /// The base event tests.
    /// </summary>
    public class BaseEventTests
    {
        #region Public Methods

        /// <summary>
        /// The properties set get correctly.
        /// </summary>
        [Fact]
        public void PropertiesSetGetCorrectly()
        {
            // Act
            var model = new BaseEvent();

            // Assert
            Assert.InRange(model.DateOccurred, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        }

        #endregion Public Methods
    }
}