namespace NFB.UI.Api
{
    using System;
    using System.Reflection;

    using Autofac;

    using MassTransit;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using NFB.Domain.Bus.Commands;
    using NFB.Domain.Settings;

    /// <summary>
    /// The startup.
    /// </summary>
    public class Startup
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <param name="env">
        /// The env.
        /// </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(
                swaggerCfg =>
                    {
                        swaggerCfg.SwaggerEndpoint("/swagger/v1/swagger.json", "NightFuryBot API");
                        swaggerCfg.RoutePrefix = string.Empty;
                    });
        }

        /// <summary>
        /// The configure container.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public void ConfigureContainer(ContainerBuilder container)
        {
            var busSettings = this.Configuration.GetSection("BusSettings").Get<BusSettings>();

            container.AddMassTransit(busConfigurator =>
                {
                    busConfigurator.AddConsumers(Assembly.GetExecutingAssembly());
                    busConfigurator.AddSagas(Assembly.GetExecutingAssembly());
                    busConfigurator.AddSagaStateMachines(Assembly.GetExecutingAssembly());

                    busConfigurator.AddRequestClient<CreateFlightCommand>(new Uri("queue:service_flight"));
                    busConfigurator.AddRequestClient<GetFlightsCommand>(new Uri("queue:service_flight"));

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
                                        });
                            });
                });
            container.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces();
        }

        /// <summary>
        /// Add services to the container.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen();
            services.AddControllers();
            services.AddMassTransitHostedService();
        }

        #endregion Public Methods
    }
}