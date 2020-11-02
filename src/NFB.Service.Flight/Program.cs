namespace NFB.Service.Flight
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using Autofac;
    using Autofac.Extensions.DependencyInjection;

    using MassTransit;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Newtonsoft.Json;

    using NFB.Domain.Settings;
    using NFB.Infrastructure.CrossCutting.Logging;
    using NFB.Service.Flight.Models;
    using NFB.Service.Flight.Repository;
    using NFB.Service.Flight.StateMachines;
    using NFB.Service.Flight.States;

    using Serilog;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        #region Private Fields

        /// <summary>
        /// The configuration.
        /// </summary>
        private static IConfigurationRoot configuration;

        #endregion Private Fields

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
        public static IHostBuilder CreateHost(string[] args)
        {
            return new HostBuilder()
                .ConfigureAppConfiguration(
                    configureDelegate =>
                        {
                            configureDelegate
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", true)
                                .AddEnvironmentVariables();

                            configuration = configureDelegate.Build();
                        })
                .ConfigureServices(
                    configureDelegate =>
                        {
                            configureDelegate.AddAutofac();
                            configureDelegate.AddSingleton(configuration);
                            configureDelegate.AddMassTransitHostedService();

                            var airportsJsonSerialized = JsonConvert.DeserializeObject<AirportRootModel>(File.ReadAllText(@"airports.json"));
                            var airportRepository = new AirportRepository(airportsJsonSerialized);
                            configureDelegate.AddSingleton(airportRepository);
                        })
                .ConfigureContainer<ContainerBuilder>(
                    (builderContext, configureDelegate) =>
                        {
                            var busSettings = configuration.GetSection("BusSettings").Get<BusSettings>();

                            configureDelegate.AddMassTransit(busConfigurator =>
                                {
                                    busConfigurator.AddRabbitMqMessageScheduler();
                                    busConfigurator.AddConsumers(Assembly.GetExecutingAssembly());
                                    busConfigurator.AddSagas(Assembly.GetExecutingAssembly());
                                    busConfigurator.AddSagaStateMachine<FlightStateMachine, FlightState>()
                                        .MongoDbRepository(
                                            r =>
                                                {
                                                    r.Connection = "mongodb://mongo";
                                                    r.DatabaseName = "service-flight-flights";
                                                });

                                    busConfigurator.UsingRabbitMq(
                                        (registrationContext, rabbitConfigurator) =>
                                            {
                                                rabbitConfigurator.UseDelayedExchangeMessageScheduler();

                                                rabbitConfigurator.Host(
                                                    new Uri(busSettings.Host),
                                                    hostConfigurator =>
                                                        {
                                                            hostConfigurator.Username(busSettings.Username);
                                                            hostConfigurator.Password(busSettings.Password);
                                                        });

                                                rabbitConfigurator.ReceiveEndpoint(
                                                    busSettings.Queue,
                                                    endpointConfigurator =>
                                                        {
                                                            endpointConfigurator.ConfigureConsumers(registrationContext);
                                                            endpointConfigurator.ConfigureSagas(registrationContext);
                                                        });
                                            });
                                });
                            configureDelegate.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces();
                        });
        }

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

            await CreateHost(args)
                .UseSerilog()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .RunConsoleAsync();
        }

        #endregion Public Methods
    }
}