namespace NFB.Service.Flight.Tests
{
    using Microsoft.Extensions.Hosting;

    using Xunit;

    /// <summary>
    /// The program tests.
    /// </summary>
    public class ProgramTests
    {
        #region Public Methods

        /// <summary>
        /// The create host returns host.
        /// </summary>
        [Fact]
        public void CreateHostReturnsHost()
        {
            // Assert
            Assert.IsType<HostBuilder>(Program.CreateHost(null));
        }

        #endregion Public Methods
    }
}