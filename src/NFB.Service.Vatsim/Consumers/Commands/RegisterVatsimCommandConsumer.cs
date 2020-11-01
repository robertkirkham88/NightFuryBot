namespace NFB.Service.Vatsim.Consumers.Commands
{
    using System;
    using System.Threading.Tasks;

    using AutoMapper;

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

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterVatsimCommandConsumer"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        public RegisterVatsimCommandConsumer(VatsimDbContext database, IMapper mapper)
        {
            this.database = database;
            this.mapper = mapper;
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
                    await this.database.Pilots.AddAsync(this.mapper.Map(context.Message, new PilotEntity()));
                else
                    this.database.Pilots.Update(this.mapper.Map(context.Message, existingEntity));

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