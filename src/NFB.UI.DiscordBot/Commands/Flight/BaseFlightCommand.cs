namespace NFB.UI.DiscordBot.Commands.Flight
{
    using Discord.Commands;

    using MassTransit;

    /// <summary>
    /// The base flight command.
    /// </summary>
    [Group("Flight")]
    [Alias("f")]
    public class BaseFlightCommand : ModuleBase
    {
        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFlightCommand"/> class.
        /// </summary>
        /// <param name="bus">
        /// The bus.
        /// </param>
        protected BaseFlightCommand(IBus bus)
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