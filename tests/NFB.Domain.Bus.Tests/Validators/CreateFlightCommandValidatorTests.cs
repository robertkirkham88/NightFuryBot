namespace NFB.Domain.Bus.Tests.Validators
{
    using System;

    using FluentValidation.TestHelper;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.Validators;

    using Xunit;

    /// <summary>
    /// The flight entity validator tests.
    /// </summary>
    public class CreateFlightCommandValidatorTests
    {
        #region Private Fields

        /// <summary>
        /// The validator.
        /// </summary>
        private readonly CreateFlightCommandValidator validator;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFlightCommandValidatorTests"/> class.
        /// </summary>
        public CreateFlightCommandValidatorTests()
        {
            this.validator = new CreateFlightCommandValidator();
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
            var entity = new CreateFlightCommand
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
            var entity = new CreateFlightCommand
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
            var entity = new CreateFlightCommand
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
            var entity = new CreateFlightCommand
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