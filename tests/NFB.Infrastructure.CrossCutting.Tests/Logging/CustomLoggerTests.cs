namespace NFB.Infrastructure.CrossCutting.Tests.Logging
{
    using NFB.Infrastructure.CrossCutting.Logging;

    using Serilog;

    using Xunit;

    /// <summary>
    /// The custom logger.
    /// </summary>
    public class CustomLoggerTests
    {
        #region Public Methods

        /// <summary>
        /// The create custom logger configuration returns logger configuration.
        /// </summary>
        [Fact]
        public void CreateCustomLoggerConfigurationReturnsLoggerConfiguration()
        {
            // Arrange
            var loggerConfiguration = CustomLogger.CreateLoggerConfiguration();

            // Assert
            Assert.IsType<LoggerConfiguration>(loggerConfiguration);
            Assert.NotNull(loggerConfiguration);
        }

        #endregion Public Methods
    }
}