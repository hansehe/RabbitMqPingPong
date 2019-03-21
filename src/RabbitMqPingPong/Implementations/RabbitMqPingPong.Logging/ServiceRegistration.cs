using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.AspNetCore;

namespace RabbitMqPingPong.Logging
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterLogging(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddLogging()
                .AddSingleton<ILoggerFactory>(services =>
                {
                    var configuration = services.GetRequiredService<IConfiguration>();
                    var logger = SerilogExtensions.CreateSerilogger(configuration);
                    return new SerilogLoggerFactory(logger);
                });
        }
    }
}