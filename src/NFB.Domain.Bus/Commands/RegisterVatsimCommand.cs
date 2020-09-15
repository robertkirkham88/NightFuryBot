namespace NFB.Domain.Bus.Commands
{
    /// <summary>
    /// The register vatsim command.
    /// </summary>
    public class RegisterVatsimCommand
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the users id.
        /// </summary>
        public string UserId { get; set; }

        #endregion Public Properties
    }
}