using Microsoft.Extensions.Configuration;

namespace RabbitMqPingPong
{
    public static class RabbitMqHelper
    {
        public static string BuildRabbitMqConnectionString(this IConfiguration configuration)
        {
            var rabbitMqSettings = configuration.GetSection("amqp");
            var user = rabbitMqSettings.GetValue<string>("user");
            var password = rabbitMqSettings.GetValue<string>("password");
            var hostname = rabbitMqSettings.GetValue<string>("hostname");
            var port = rabbitMqSettings.GetValue<string>("port");
            var virtualHost = rabbitMqSettings.GetValue<string>("virtualhost");
            
            var connectionString =$"amqp://{user}:{password}@{hostname}:{port}/{virtualHost}";
            return connectionString;
        }
    }
}