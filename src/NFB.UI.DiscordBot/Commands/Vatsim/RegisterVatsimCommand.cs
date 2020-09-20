namespace NFB.UI.DiscordBot.Commands.Vatsim
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    using Discord.Commands;

    using MassTransit;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.Responses;

    /// <summary>
    /// The register vatsim id command.
    /// </summary>
    public class RegisterVatsimIdCommand : BaseVatsimCommand
    {
        #region Private Fields

        /// <summary>
        /// The request.
        /// </summary>
        private readonly IRequestClient<RegisterVatsimCommand> request;

        #endregion Private Fields

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterVatsimIdCommand"/> class.
        /// </summary>
        /// <param name="bus">
        /// The bus.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        protected RegisterVatsimIdCommand(IBus bus, IRequestClient<RegisterVatsimCommand> request)
            : base(bus)
        {
            this.request = request;
        }

        #endregion Protected Constructors

        #region Public Methods

        /// <summary>
        /// Register user to a vatsim id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Name("Register Vatsim ID")]
        [Command]
        [Summary("!vatsim [ID]\r\nRegister your Vatsim to the specified ID.\r\nExample:\r\n!vatsim 123456789")]
        public async Task<RuntimeResult> ExecuteAsync(string id)
        {
            try
            {
                var (successful, failed) =
                    await this.request
                        .GetResponse<RegisterVatsimCommandSuccessResponse, RegisterVatsimCommandFailResponse>(
                            new RegisterVatsimCommand { Id = id, UserId = this.Context.User.Id.ToString() });

                if (successful.IsCompletedSuccessfully)
                {
                    await successful;
                    return CommandResult.FromSuccess($"Successfully registered your Vatsim ID {id}");
                }

                await failed;
                return CommandResult.FromError($"Failed to register your Vatsim ID as {id}");
            }
            catch (Exception ex)
            {
                return CommandResult.FromError(ex.Message);
            }
        }

        #endregion Public Methods
    }
}