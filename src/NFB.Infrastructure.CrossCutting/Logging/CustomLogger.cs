namespace NFB.Infrastructure.CrossCutting.Logging
{
    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// The custom logger.
    /// </summary>
    public static class CustomLogger
    {
        #region Public Methods

        /// <summary>
        /// The custom logger configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="LoggerConfiguration"/>.
        /// </returns>
        public static LoggerConfiguration CreateLoggerConfiguration()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.FromMassTransit()
                .WriteTo.Seq("http://nfb.logging:5341")
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}");
        }

        #endregion Public Methods
    }
}