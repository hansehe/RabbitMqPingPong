using System;
using RabbitMqPingPong.Migration.MigrationModels;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.Migration;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMqPingPong.Migration
{
    public static class MigrationExecutor
    {
        public static IServiceProvider MigrateUpSchema(this IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            configuration.MigrateUpSchema();
            return serviceProvider;
        }
        
        public static IConfiguration MigrateUpSchema(this IConfiguration configuration)
        {
            return configuration
                .MigrateUpServiceSchema();
        }
        
        public static IConfiguration MigrateUpServiceSchema(this IConfiguration configuration)
        {
            new ServiceCollection()
                .AddSingleton(sp => configuration)
                .RegisterServiceMigration()
                .BuildServiceProvider()
                .MigrateUp();

            return configuration;
        }
        
        public static IServiceCollection RegisterServiceMigration(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .ConfigureWithMigration(new[] {typeof(AddEventTable).Assembly})
                .AddTransient<IDbConfig, MSDbConfig>();
        }
    }
}