namespace NFB.Service.Vatsim.Services
{
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using MassTransit;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;

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
        public PilotService(VatsimDbContext database, IBusControl bus)
        {
            this.database = database;
            this.bus = bus;
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
            var jsonData = new WebClient().DownloadString("http://cluster.data.vatsim.net/vatsim-data.json");
            var databasePilotsIds = await this.database.Pilots.Select(p => p.VatsimId).ToListAsync();
            var onlinePilots = JsonConvert.DeserializeObject<PilotRootModel>(jsonData).Pilots.Where(p => databasePilotsIds.Contains(p.Cid));

            foreach (var pilot in onlinePilots)
            {
                await this.bus.Publish(new VatsimPilotUpdated
                {
                    DestinationAirport = pilot.PlannedDestinationAirport,
                    OriginAirport = pilot.PlannedDepartureAirport,
                    Latitude = pilot.Latitude,
                    Longitude = pilot.Longitude,
                    VatsimId = pilot.Cid
                });
            }
        }

        #endregion Private Methods
    }
}