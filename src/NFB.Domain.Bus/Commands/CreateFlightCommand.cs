namespace NFB.Domain.Bus.Commands
{
    using System;

    /// <summary>
    /// The create flight command.
    /// </summary>
    public class CreateFlightCommand
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Gets or sets the origin.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        #endregion Public Properties
    }
}