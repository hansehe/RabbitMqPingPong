using Serilog.Core;
using Serilog.Events;

namespace RabbitMqPingPong.Logging
{
    internal class Log4NetLevelEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var log4NetLevel = Map(logEvent.Level);
            logEvent.AddPropertyIfAbsent(new LogEventProperty("Log4NetLevel", new ScalarValue(log4NetLevel)));
        }

        private string Map(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Verbose: return "TRACE";
                case LogEventLevel.Debug: return "DEBUG";
                case LogEventLevel.Information: return "INFO";
                case LogEventLevel.Warning: return "WARN";
                case LogEventLevel.Error: return "ERROR";
                case LogEventLevel.Fatal: return "FATAL";
                default: return string.Empty;
            }
        }

    }
}