using System.Collections.Generic;
using FluentDbTools.Extensions.MSDependencyInjection;
using Microsoft.Extensions.Configuration;

namespace RabbitMqPingPong.Config
{
    public static class ConfigBuilderExtensions
    {
        public static IConfiguration GetConfiguration(Dictionary<string, string> overrideConfig = null)
        {
            var configuration = GetConfigurationBuilder(overrideConfig)
                .Build();
            return configuration;
        }
        
        public static IConfigurationBuilder GetConfigurationBuilder(Dictionary<string, string> overrideConfig = null)
        {
            return new ConfigurationBuilder()
                .AddConfiguration(overrideConfig);
        }
        
        public static IConfiguration GetDefaultConfiguration(Dictionary<string, string> overrideConfig = null)
        {
            return new ConfigurationBuilder()
                .AddJsonFile(BaseConfig.DefaultConfigFilename)
                .AddJsonFileIfTrue(BaseConfig.DefaultConfigDockerFilename, () => BaseConfig.InContainer)
                .AddInMemoryIfTrue(overrideConfig, () => overrideConfig != null)
                .Build();
        }
        
        public static IConfigurationBuilder AddConfiguration(
            this IConfigurationBuilder configurationBuilder, 
            Dictionary<string, string> overrideConfig = null)
        {
            var defaultConfiguration = GetDefaultConfiguration(overrideConfig);

            return configurationBuilder
                .AddConfiguration(defaultConfiguration);
        }
    }
}