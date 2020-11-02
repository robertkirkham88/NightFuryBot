namespace NFB.Service.Vatsim.Consumers.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using MassTransit;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Bus.Responses;
    using NFB.Service.Vatsim.Entities;
    using NFB.Service.Vatsim.Repositories;

    /// <summary>
    /// The register vatsim command consumer.
    /// </summary>
    public class RegisterVatsimCommandConsumer : IConsumer<RegisterVatsimCommand>
    {
        #region Private Fields

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IPilotRepository repository;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterVatsimCommandConsumer"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        public RegisterVatsimCommandConsumer(IPilotRepository repository, IMapper mapper)
        {
            this.repository = repository;
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
                var entities = await this.repository.Get(p => p.UserId == context.Message.UserId);
                var existingEntity = entities.FirstOrDefault();

                if (existingEntity == null)
                    await this.repository.Create(this.mapper.Map(context.Message, new PilotEntity()));
                else
                    await this.repository.Update(existingEntity.Id, this.mapper.Map(context.Message, existingEntity));

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