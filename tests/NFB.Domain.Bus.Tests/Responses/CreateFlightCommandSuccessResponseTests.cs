namespace NFB.Domain.Bus.Tests.Responses
{
    using System;

    using NFB.Domain.Bus.Responses;

    using Xunit;

    /// <summary>
    /// The create flight command success response.
    /// </summary>
    public class CreateFlightCommandSuccessResponseTests
    {
        #region Public Methods

        /// <summary>
        /// The properties set get correctly.
        /// </summary>
        [Fact]
        public void PropertiesSetGetCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var response = new CreateFlightCommandSuccessResponse
            {
                Id = id
            };

            // Assert
            Assert.Equal(id, response.Id);
        }

        #endregion Public Methods
    }
}