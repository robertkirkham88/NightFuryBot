namespace NFB.Domain.Tests.Models
{
    using System;

    using NFB.Domain.Models;

    using Xunit;

    /// <summary>
    /// The base entity tests.
    /// </summary>
    public class BaseEntityTests
    {
        #region Public Methods

        /// <summary>
        /// The properties get set correctly.
        /// </summary>
        [Fact]
        public void PropertiesGetSetCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var model = new BaseEntity
            {
                Id = id
            };

            // Assert
            Assert.Equal(id, model.Id);
            Assert.NotNull(model.Events); // The list is initialized.
        }

        #endregion Public Methods
    }
}