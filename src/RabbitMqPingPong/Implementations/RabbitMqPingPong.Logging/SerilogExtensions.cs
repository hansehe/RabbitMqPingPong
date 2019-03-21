using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace RabbitMqPingPong.Logging
{
    public static class SerilogExtensions
    {
        public static ILogger CreateSerilogger(IConfiguration configuration)
        {
            var loggerConfiguration = new LoggerConfiguration();
            ConfigureSerilogger(configuration, loggerConfiguration);
            return loggerConfiguration.CreateLogger();
        }
        
        private static void ConfigureSerilogger(IConfiguration configuration, LoggerConfiguration loggerConfiguration)
        {
            var outputFolder = configuration.GetValue("logPath", ApplicationConstants.LogPath);
            var outputLogFile = Path.Combine(outputFolder,
                ApplicationConstants.ApplicationName + "-Main-{Date}.log");
            var outputProfilingFile = Path.Combine(outputFolder,
                ApplicationConstants.ApplicationName + "-Profiling-{Date}.log");
                
            const string outputTemplate =
                "{Timestamp:yyyyMMdd HH:mm:ss,fff};[{Log4NetLevel}];{ApplicationId};{SessionId};{CallId};{ThreadId};{SourceContext:l};{Message:lj}{NewLine}{Exception}";

            var profilingFilter = Matching.FromSource(ApplicationConstants.ProfilingMatch);

            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .ReadFrom.Configuration(configuration)
                .WriteTo.Logger(config => config
                    .WriteTo.Console(outputTemplate: outputTemplate)
                    .WriteTo.RollingFile(outputLogFile,
                        outputTemplate: outputTemplate)
                    .Filter.ByExcluding(profilingFilter)
                    .Enrich.FromLogContext()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("ApplicationId", ApplicationConstants.ApplicationId)
                    .Enrich.WithProperty("SessionId", ApplicationConstants.SessionId)
                    .Enrich.WithLog4NetLevel()
                )
                .WriteTo.Logger(config => config
                    .WriteTo.RollingFile(outputProfilingFile,
                        outputTemplate: "{Message}{NewLine}")
                    .Filter.ByIncludingOnly(profilingFilter)
                );
        }
    }
}