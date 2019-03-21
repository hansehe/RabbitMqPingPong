using System;
using Serilog;
using Serilog.Configuration;

namespace RabbitMqPingPong.Logging
{
    internal static class Log4NetLevelLoggerConfigurationExtensions
    {
        public static LoggerConfiguration WithLog4NetLevel(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With<Log4NetLevelEnricher>();
        }
    }
}