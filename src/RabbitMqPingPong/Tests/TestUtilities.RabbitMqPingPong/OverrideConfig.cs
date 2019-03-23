using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMqPingPong;
using RabbitMqPingPong.Config;
using FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestUtilities.RabbitMqPingPong
{
    public static class OverrideConfig
    {
        private static string RandomQueue => $"InputQueue_{new Random().Next().ToString()}";
        private static string RandomExchange => $"Exchange_{new Random().Next().ToString()}";
        public const SupportedDatabaseTypes DefaultDatabaseType = SupportedDatabaseTypes.Postgres;
        
        public static Dictionary<string, string> GetOverrideConfig(
            SupportedDatabaseTypes databaseType = DefaultDatabaseType, 
            string user = null)
        {
            user = user ?? DatabaseMigrationFixture.MigratedDatabaseUser;
            
            var randomQueue = RandomQueue;
            var overrideDict = new Dictionary<string, string>
            {
                {"database:user", user},
                {"amqp:inputQueue", randomQueue},
                {"mqtt:inputQueue", randomQueue},
                {"amqp:directExchange", RandomExchange},
                {"amqp:topicExchange", RandomExchange},
                {"centralConfig:enable", false.ToString()}
            };
            
            switch (databaseType)
            {
                case SupportedDatabaseTypes.Postgres:
                    overrideDict["database:type"] = "postgres";
                    overrideDict["database:adminUser"] = "admin";
                    overrideDict["database:adminPassword"] = "admin";
                    overrideDict["database:databaseConnectionName"] = user;
                    overrideDict["database:hostname"] = BaseConfig.InContainer ? "postgres-db" : "localhost";
                    overrideDict["database:port"] = BaseConfig.InContainer ? "5432" : "5433";
                    break;
                case SupportedDatabaseTypes.Oracle:
                    overrideDict["database:type"] = "oracle";
                    overrideDict["database:adminUser"] = "system";
                    overrideDict["database:adminPassword"] = "oracle";
                    overrideDict["database:databaseConnectionName"] = "xe";
                    overrideDict["database:hostname"] = BaseConfig.InContainer ? "oracle-db" : "localhost";
                    overrideDict["database:port"] = "1521";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }
            
            return overrideDict;
        }

        public static IConfiguration GetDefaultConfiguration(SupportedDatabaseTypes databaseType = DefaultDatabaseType, 
            Dictionary<string, string> additionalOverrideConfig = null)
        {
            var overrideConfig = GetOverrideConfig(databaseType);
            additionalOverrideConfig?.ToList().ForEach(x => overrideConfig[x.Key] = x.Value);
            var configuration =
                ConfigBuilderExtensions.GetDefaultConfiguration(overrideConfig);
            return configuration;
        }
        
        public static IServiceCollection GetServiceCollection(
            SupportedDatabaseTypes databaseType = DefaultDatabaseType,
            Dictionary<string, string> additionalOverrideConfig = null)
        {
            var configuration = GetDefaultConfiguration(databaseType, additionalOverrideConfig);
            return new ServiceCollection()
                .RegisterService(configuration);
        }
    }
}