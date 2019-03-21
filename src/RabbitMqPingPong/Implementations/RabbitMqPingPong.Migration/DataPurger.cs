using System;
using FluentDbTools.Extensions.Migration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMqPingPong.Migration
{
    public static class DataPurger
    {
        public static IServiceProvider DropServiceSchema(this IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            configuration.DropServiceSchema();
            return serviceProvider;
        }

        public static IConfiguration DropServiceSchema(this IConfiguration configuration)
        {
            new ServiceCollection()
                .AddSingleton(sp => configuration)
                .RegisterServiceMigration()
                .BuildServiceProvider()
                .DropSchema();
            
            return configuration;
        }
    }
}