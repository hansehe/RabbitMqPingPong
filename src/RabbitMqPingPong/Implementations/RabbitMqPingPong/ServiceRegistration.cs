using System.Security.Cryptography.X509Certificates;
using RabbitMqPingPong.Abstractions;
using RabbitMqPingPong.Contracts;
using RabbitMqPingPong.Database.Repositories;
using RabbitMqPingPong.LoggerAdapter;
using RabbitMqPingPong.Logging;
using RabbitMqPingPong.MessageHandlers;
using RabbitMqPingPong.Mqtt;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;
using FluentDbTools.Extensions.MSDependencyInjection.Oracle;
using FluentDbTools.Extensions.MSDependencyInjection.Postgres;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;
using Rebus.Retry.Simple;
using Rebus.ServiceProvider;
using uPLibrary.Networking.M2Mqtt;

namespace RabbitMqPingPong
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterService(this IServiceCollection serviceCollection, 
            IConfiguration configuration)
        {
            return serviceCollection
                .AddSingleton(sp => configuration)
                .RegisterLogging()
                .RegisterDatabase()
                .RegisterDatabaseRepositories()
                .RegisterMessageHandlers()
                .RegisterRebusHealthCheck(configuration)
                .RegisterRebusLogger()
                .RegisterRebus()
                .RegisterMqtt();
        }
        
        private static IServiceCollection RegisterRebusLogger(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddTransient<IRebusLoggerFactory, MsRebusLoggerFactory>();
            serviceCollection.TryAddScoped(provider => provider.GetRequiredService<IRebusLoggerFactory>().GetLogger<MsRebusLogger>());
            return serviceCollection;
        }
        
        private static IServiceCollection RegisterRebusHealthCheck(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddHealthChecks()
                .AddRabbitMQ(configuration.BuildRabbitMqConnectionString());
            return serviceCollection;
        }

        private static IServiceCollection RegisterRebus(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddRebus((configure, serviceProvider) =>
                {
                    var configuration = serviceProvider.GetService<IConfiguration>();
                    var loggerFactory = serviceProvider.GetService<IRebusLoggerFactory>();

                    var rabbitMqSettings = configuration.GetSection("amqp");
                    var inputQueue = rabbitMqSettings.GetValue<string>("inputQueue");
                    var errorQueue = rabbitMqSettings.GetValue<string>("inputQueue");
                    var directExchange = rabbitMqSettings.GetValue<string>("directExchange");
                    var topicExchange = rabbitMqSettings.GetValue<string>("topicExchange");

                    var connectionString = configuration.BuildRabbitMqConnectionString();

                    return configure
                        .Logging(l => l.Use(loggerFactory))
                        .Options(optionsConfigure =>
                        {
                            optionsConfigure.SetNumberOfWorkers(2);
                            optionsConfigure.SetMaxParallelism(10);
                            optionsConfigure.SimpleRetryStrategy(errorQueue);
                        })
                        .Transport(t =>
                            t.UseRabbitMq(connectionString, inputQueue)
                                .EnablePublisherConfirms()
                                .ExchangeNames(directExchange, topicExchange));
                });
        }
        
        private static IServiceCollection RegisterMqtt(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IMqttPublisher, MqttPublisher>()
                .AddSingleton(sp =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    var mqttConfigSection = configuration.GetSection("mqtt");
                    var mqttSslConfigSection = mqttConfigSection.GetSection("ssl");

                    var certificate = new X509Certificate(
                        mqttSslConfigSection.GetValue<string>("certificatePath"),
                        mqttSslConfigSection.GetValue<string>("certificatePassphrase"));
                    
                    var client = new MqttClient(
                        mqttConfigSection.GetValue<string>("hostname"), 
                        mqttConfigSection.GetValue<int>("port"), 
                        mqttSslConfigSection.GetValue<bool>("enable"), 
                        certificate, certificate, MqttSslProtocols.None, 
                        (sender, x509Certificate, chain, errors) => true);
                    return client;
                });
        }
        
        private static IServiceCollection RegisterDatabase(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IDbConfig, MSDbConfig>()
                .AddPostgresDbProvider()
                .AddOracleDbProvider();
        }
        
        private static IServiceCollection RegisterMessageHandlers(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IHandleMessages<EventContract>, EventMessageHandler>();
        }
        
        private static IServiceCollection RegisterDatabaseRepositories(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IEventRepository, EventRepository>();
        }
    }
}