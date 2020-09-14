namespace NFB.Service.Flight.Tests.Validators
{
    using System;

    using FluentValidation.TestHelper;

    using NFB.Service.Flight.Entities;
    using NFB.Service.Flight.Validators;

    using Xunit;

    /// <summary>
    /// The flight entity validator tests.
    /// </summary>
    public class FlightEntityValidatorTests
    {
        #region Private Fields

        /// <summary>
        /// The validator.
        /// </summary>
        private readonly FlightEntityValidator validator;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightEntityValidatorTests"/> class.
        /// </summary>
        public FlightEntityValidatorTests()
        {
            this.validator = new FlightEntityValidator();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// There is an error when destination length incorrect.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        [Theory]
        [InlineData("EGL")]
        [InlineData("EGLLL")]
        [InlineData(null)]
        public void ErrorWhenDestinationLengthIncorrect(string destination)
        {
            // Arrange
            var entity = new FlightEntity
            {
                Origin = "EGCC",
                Destination = destination,
                StartTime = DateTime.UtcNow.AddHours(3)
            };

            // Act
            var result = this.validator.TestValidate(entity);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.Destination);
        }

        /// <summary>
        /// There is an error when origin length incorrect.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        [Theory]
        [InlineData("EGL")]
        [InlineData("EGLLL")]
        [InlineData(null)]
        public void ValidationErrorWhenOriginLengthIncorrect(string origin)
        {
            // Arrange
            var entity = new FlightEntity
            {
                Origin = origin,
                Destination = "EGCC",
                StartTime = DateTime.UtcNow.AddHours(3)
            };

            // Act
            var result = this.validator.TestValidate(entity);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.Origin);
        }

        /// <summary>
        /// Validation error when start date in past incorrect.
        /// </summary>
        [Fact]
        public void ValidationErrorWhenStartDateInPastIncorrect()
        {
            // Arrange
            var entity = new FlightEntity
            {
                Origin = "EGLL",
                Destination = "EGCC",
                StartTime = DateTime.UtcNow.AddSeconds(10)
            };

            // Act
            var result = this.validator.TestValidate(entity);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.StartTime);
        }

        /// <summary>
        /// Validator should not have any validation errors.
        /// </summary>
        [Fact]
        public void ValidatorShouldNotHaveAnyValidationErrors()
        {
            // Arrange
            var entity = new FlightEntity
            {
                Destination = "EGCC",
                Origin = "EGLL",
                StartTime = DateTime.UtcNow.AddHours(3)
            };

            // Act
            var result = this.validator.TestValidate(entity);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        #endregion Public Methods
    }
}