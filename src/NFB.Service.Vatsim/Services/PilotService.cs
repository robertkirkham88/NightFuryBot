namespace NFB.Service.Vatsim.Services
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using NFB.Domain.Bus.Events;
    using NFB.Service.Vatsim.Models;
    using NFB.Service.Vatsim.Persistence;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The pilot service.
    /// </summary>
    public class PilotService : IHostedService
    {
        #region Private Fields

        /// <summary>
        /// The bus.
        /// </summary>
        private readonly IBusControl bus;

        /// <summary>
        /// The database.
        /// </summary>
        private readonly VatsimDbContext database;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<PilotService> logger;

        /// <summary>
        /// The timer.
        /// </summary>
        private readonly Timer timer;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotService"/> class.
        /// </summary>
        /// <param name="database">
        /// The database.
        /// </param>
        /// <param name="bus">
        /// The bus.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public PilotService(VatsimDbContext database, IBusControl bus, ILogger<PilotService> logger)
        {
            this.database = database;
            this.bus = bus;
            this.logger = logger;
            this.timer = new Timer
            {
                AutoReset = true,
                Enabled = false,
                Interval = 60000
            };
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Start the service.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.timer.Start();
            this.timer.Elapsed += this.UpdatePilotInformation;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stop the service.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.timer.Elapsed -= this.UpdatePilotInformation;
            this.timer.Stop();
            return Task.CompletedTask;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Update the pilot information.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void UpdatePilotInformation(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                var jsonData = new WebClient().DownloadString("http://cluster.data.vatsim.net/vatsim-data.json");

                var databasePilotsIds = await this.database.Pilots.ToListAsync();
                var onlinePilots = JsonConvert.DeserializeObject<PilotRootModel>(jsonData).Pilots;

                foreach (var pilot in databasePilotsIds)
                {
                    var isOnline = onlinePilots.FirstOrDefault(p => p.Cid == pilot.VatsimId);

                    if (isOnline != null)
                    {
                        await this.bus.Publish(
                            new VatsimPilotUpdatedEvent
                            {
                                DestinationAirport = isOnline.PlannedDestinationAirport,
                                OriginAirport = isOnline.PlannedDepartureAirport,
                                Latitude = isOnline.Latitude,
                                Longitude = isOnline.Longitude,
                                VatsimId = isOnline.Cid,
                                UserId = ulong.Parse(pilot.UserId)
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex.Message);
            }
        }

        #endregion Private Methods
    }
}