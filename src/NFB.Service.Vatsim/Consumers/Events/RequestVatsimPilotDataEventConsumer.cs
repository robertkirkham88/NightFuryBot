namespace NFB.Service.Vatsim.Consumers.Events
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using AutoMapper;

    using MassTransit;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using NFB.Domain.Bus.Events;
    using NFB.Service.Vatsim.Models;
    using NFB.Service.Vatsim.Repositories;

    /// <summary>
    /// The request vatsim pilot data event consumer.
    /// </summary>
    public class RequestVatsimPilotDataEventConsumer : IConsumer<RequestVatsimPilotDataEvent>
    {
        #region Private Fields

        /// <summary>
        /// The bus.
        /// </summary>
        private readonly IBusControl bus;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<RequestVatsimPilotDataEventConsumer> logger;

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
        /// Initializes a new instance of the <see cref="RequestVatsimPilotDataEventConsumer"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="bus">
        /// The bus.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="mapper">
        /// The mapper.
        /// </param>
        public RequestVatsimPilotDataEventConsumer(IPilotRepository repository, IBusControl bus, ILogger<RequestVatsimPilotDataEventConsumer> logger, IMapper mapper)
        {
            this.repository = repository;
            this.bus = bus;
            this.logger = logger;
            this.mapper = mapper;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Consume the message.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Consume(ConsumeContext<RequestVatsimPilotDataEvent> context)
        {
            try
            {
                var jsonData = new WebClient().DownloadString("http://cluster.data.vatsim.net/vatsim-data.json");

                var databasePilotsIds = await this.repository.Get();
                var onlinePilots = JsonConvert.DeserializeObject<PilotRootModel>(jsonData).Pilots;

                foreach (var pilot in databasePilotsIds)
                {
                    var isOnline = onlinePilots.FirstOrDefault(p => p.Cid == pilot.VatsimId);

                    if (isOnline != null)
                    {
                        await this.bus.Publish(
                            this.mapper.Map(
                                isOnline,
                                new VatsimPilotUpdatedEvent
                                {
                                    UserId = ulong.Parse(pilot.UserId),
                                    Status = "Online"
                                }));
                    }
                    else
                    {
                        await this.bus.Publish(new VatsimPilotUpdatedEvent
                        {
                            UserId = ulong.Parse(pilot.UserId),
                            Status = "Offline"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex.Message);
            }
        }

        #endregion Public Methods
    }
}