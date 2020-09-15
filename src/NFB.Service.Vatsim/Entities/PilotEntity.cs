namespace NFB.Service.Vatsim.Entities
{
    using System;

    /// <summary>
    /// The vatsim entity.
    /// </summary>
    public class PilotEntity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string VatsimId { get; set; }

        #endregion Public Properties
    }
}