namespace NFB.Service.Flight.Validators
{
    using System;

    using FluentValidation;

    using NFB.Service.Flight.Entities;

    /// <summary>
    /// The flight entity validator.
    /// </summary>
    public class FlightEntityValidator : AbstractValidator<FlightEntity>
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightEntityValidator"/> class.
        /// </summary>
        public FlightEntityValidator()
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