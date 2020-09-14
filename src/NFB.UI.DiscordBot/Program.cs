﻿namespace NFB.UI.DiscordBot
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using Autofac;
    using Autofac.Extensions.DependencyInjection;

    using MassTransit;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Settings;
    using NFB.Infrastructure.CrossCutting.Logging;
    using NFB.Ui.DiscordBot.Extensions;
    using NFB.UI.DiscordBot.Persistence;
    using NFB.UI.DiscordBot.StateMachines;
    using NFB.UI.DiscordBot.States;

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

                            var databaseConnection = configuration.GetConnectionString("db");
                            configureDelegate.AddDbContext<DiscordBotDbContext>(options => options.UseNpgsql(databaseConnection));

                            var serviceProvider = configureDelegate.BuildServiceProvider();
                            using (var databaseContext = serviceProvider.GetRequiredService<DiscordBotDbContext>())
                            {
                                databaseContext.Database.Migrate();
                            }
                        })
                .ConfigureContainer<ContainerBuilder>(
                    (builderContext, configureDelegate) =>
                        {
                            configureDelegate.AddCommandService();
                            configureDelegate.AddDiscordService();

                            var busSettings = configuration.GetSection("BusSettings").Get<BusSettings>();
                            configureDelegate.AddMassTransit(
                                busConfigurator =>
                                    {
                                        busConfigurator.AddConsumers(Assembly.GetExecutingAssembly());
                                        busConfigurator.AddSagas(Assembly.GetExecutingAssembly());
                                        busConfigurator.AddSagaStateMachine<FlightStateMachine, FlightState>().MartenRepository(configuration.GetConnectionString("db"));

                                        busConfigurator.AddRequestClient<CreateFlightCommand>(new Uri($"{busSettings.Host}/service_flight"));

                                        busConfigurator.UsingRabbitMq(
                                            (registrationContext, rabbitConfigurator) =>
                                                {
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

                                                                endpointConfigurator.UseMessageRetry(p => p.SetRetryPolicy(f => f.Interval(5, TimeSpan.FromSeconds(1))));
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