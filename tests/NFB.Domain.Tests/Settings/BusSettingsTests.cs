namespace NFB.Domain.Tests.Settings
{
    using NFB.Domain.Settings;

    using Xunit;

    /// <summary>
    /// The bus settings tests.
    /// </summary>
    public class BusSettingsTests
    {
        #region Public Methods

        /// <summary>
        /// The properties get set correctly.
        /// </summary>
        [Fact]
        public void PropertiesGetSetCorrectly()
        {
            // Arrange
            var host = "host";
            var queue = "queue";
            var username = "username";
            var password = "password";

            // Act
            var settings = new BusSettings
            {
                Host = host,
                Queue = queue,
                Password = password,
                Username = username
            };

            // Assert
            Assert.Equal(host, settings.Host);
            Assert.Equal(queue, settings.Queue);
            Assert.Equal(password, settings.Password);
            Assert.Equal(username, settings.Username);
        }

        #endregion Public Methods
    }
}