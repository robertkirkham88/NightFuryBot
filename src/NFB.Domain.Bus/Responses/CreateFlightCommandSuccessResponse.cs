namespace NFB.Domain.Bus.Responses
{
    using System;

    /// <summary>
    /// The create flight command success.
    /// </summary>
    public class CreateFlightCommandSuccessResponse
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public Guid Id { get; set; }

        #endregion Public Properties
    }
}