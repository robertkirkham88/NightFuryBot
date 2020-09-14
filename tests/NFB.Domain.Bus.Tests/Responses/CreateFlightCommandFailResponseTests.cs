namespace NFB.Domain.Bus.Tests.Responses
{
    using NFB.Domain.Bus.Responses;

    using Xunit;

    /// <summary>
    /// The create flight command success response.
    /// </summary>
    public class CreateFlightCommandFailResponseTests
    {
        #region Public Methods

        /// <summary>
        /// The properties set get correctly.
        /// </summary>
        [Fact]
        public void PropertiesSetGetCorrectly()
        {
            // Arrange
            var reason = "Something bad happened...";

            // Act
            var response = new CreateFlightCommandFailResponse
            {
                Reason = reason
            };

            // Assert
            Assert.Equal(reason, response.Reason);
        }

        #endregion Public Methods
    }
}