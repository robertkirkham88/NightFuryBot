namespace NFB.Service.Vatsim.Consumers.Commands
{
    using System;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.EntityFrameworkCore;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.Responses;
    using NFB.Service.Vatsim.Entities;
    using NFB.Service.Vatsim.Persistence;

    /// <summary>
    /// The register vatsim command consumer.
    /// </summary>
    public class RegisterVatsimCommandConsumer : IConsumer<RegisterVatsimCommand>
    {
        #region Private Fields

        /// <summary>
        /// The database.
        /// </summary>
        private readonly VatsimDbContext database;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterVatsimCommandConsumer"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        public RegisterVatsimCommandConsumer(VatsimDbContext database)
        {
            this.database = database;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// The consume.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Consume(ConsumeContext<RegisterVatsimCommand> context)
        {
            try
            {
                var existingEntity = await this.database.Pilots.FirstOrDefaultAsync(p => p.UserId == context.Message.UserId);
                if (existingEntity == null)
                {
                    await this.database.Pilots.AddAsync(new PilotEntity
                    {
                        VatsimId = context.Message.Id,
                        UserId = context.Message.UserId
                    });
                }
                else
                {
                    existingEntity.VatsimId = context.Message.Id;
                    this.database.Pilots.Update(existingEntity);
                }

                await this.database.SaveChangesAsync();
                await context.RespondAsync(new RegisterVatsimCommandSuccessResponse());
            }
            catch (Exception ex)
            {
                await context.RespondAsync(new RegisterVatsimCommandFailResponse { Reason = ex.Message });
            }
        }

        #endregion Public Methods
    }
}