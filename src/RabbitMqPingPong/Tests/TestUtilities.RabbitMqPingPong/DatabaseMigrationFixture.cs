using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMqPingPong.Config;
using RabbitMqPingPong.Migration;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.DbProvider;
using FluentDbTools.Extensions.MSDependencyInjection.DefaultConfigs;

namespace TestUtilities.RabbitMqPingPong
{  
    public class DatabaseMigrationFixture : IDisposable
    {
        public static string MigratedDatabaseUser = "RabbitMqPingPong";
        
        public DatabaseMigrationFixture()
        {
            BaseConfig.UpdateCaCertificates();
            MigratedDatabaseUser = $"RabbitMqPingPong_{new Random().Next().ToString()}";
            foreach (var databaseType in SelectedDatabaseTypesToTest())
            {
                MigrateData(databaseType);
            }
        }
        
        public void Dispose()
        {
            foreach (var databaseType in SelectedDatabaseTypesToTest())
            {
                var configuration = OverrideConfig.GetDefaultConfiguration(databaseType);
                try
                {
                    configuration.DropServiceSchema();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private static void MigrateData(SupportedDatabaseTypes databaseType)
        {
            var configuration = OverrideConfig.GetDefaultConfiguration(databaseType);
            configuration.MigrateUpSchema();
        }
        
        private static IEnumerable<SupportedDatabaseTypes> SelectedDatabaseTypesToTest()
        {
            return TestParameters.DbParameters.Select(dbParameter => (SupportedDatabaseTypes) dbParameter.First()).ToList();
        }
    }
}