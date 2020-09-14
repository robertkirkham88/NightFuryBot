namespace NFB.Domain.Bus.Validators
{
    using System;

    using FluentValidation;

    using NFB.Domain.Bus.Commands;

    /// <summary>
    /// The create flight command validator.
    /// </summary>
    public class CreateFlightCommandValidator : AbstractValidator<CreateFlightCommand>
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFlightCommandValidator"/> class.
        /// </summary>
        public CreateFlightCommandValidator()
        {
            this.RuleFor(p => p.Destination)
                .Length(4)
                .NotNull();

            this.RuleFor(p => p.Origin)
                .Length(4)
                .NotNull();

            this.RuleFor(p => p.StartTime)
                .Must(v => v > DateTime.UtcNow.AddSeconds(15))
                .NotNull();
        }

        #endregion Public Constructors
    }
}