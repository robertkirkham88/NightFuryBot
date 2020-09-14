namespace NFB.UI.Api
{
    using System.Threading.Tasks;

    using Autofac.Extensions.DependencyInjection;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    using NFB.Infrastructure.CrossCutting.Logging;

    using Serilog;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        #region Public Methods

        /// <summary>
        /// Create a new host.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IHostBuilder"/>.
        /// </returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task Main(string[] args)
        {
            Log.Logger = CustomLogger.CreateLoggerConfiguration().CreateLogger();

            await CreateHostBuilder(args)
                .UseSerilog()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .Build()
                .RunAsync();

            CreateHostBuilder(args).Build().Run();
        }

        #endregion Public Methods
    }
}