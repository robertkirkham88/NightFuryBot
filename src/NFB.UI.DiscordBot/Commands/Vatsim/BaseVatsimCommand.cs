namespace NFB.UI.DiscordBot.Commands.Vatsim
{
    using Discord.Commands;

    using MassTransit;

    /// <summary>
    /// The base vatsim command.
    /// </summary>
    [Group("Vatsim")]
    [Alias("v")]
    public class BaseVatsimCommand : ModuleBase
    {
        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseVatsimCommand"/> class.
        /// </summary>
        /// <param name="bus">
        /// The bus.
        /// </param>
        protected BaseVatsimCommand(IBus bus)
        {
            this.Bus = bus;
        }

        #endregion Protected Constructors

        #region Public Properties

        /// <summary>
        /// Gets the bus.
        /// </summary>
        public IBus Bus { get; }

        #endregion Public Properties
    }
}