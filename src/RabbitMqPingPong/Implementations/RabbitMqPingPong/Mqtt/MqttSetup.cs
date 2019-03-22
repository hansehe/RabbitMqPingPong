using System;
using System.Text;
using System.Threading;
using RabbitMqPingPong.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rebus.Bus;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace RabbitMqPingPong.Mqtt
{
    public static class MqttSetup
    {
        private static readonly string[] MqttTopics = 
        {
            EventContract.Topic
        };

        public static MqttClient Setup(this MqttClient mqttClient, IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<MqttClient>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            if (!mqttClient.Connect(configuration))
            {
                const string errorMsg = "MqttClient could not connect!";
                logger.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            
            mqttClient.ConnectionClosed += (sender, args) =>
            {
                logger.LogInformation("Connection closed");

                while (!mqttClient.TryConnect(configuration, logger))
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            };
            
            mqttClient.MqttMsgSubscribed += (sender, args) =>
            {
                logger.LogInformation("Subscribed");
            };
            
            mqttClient.MqttMsgPublished += (sender, args) =>
            {
                logger.LogInformation("Published");
            };
            
            mqttClient.MqttMsgPublishReceived += (sender, args) =>
            {
                logger.LogInformation("Publish Received");
            };
            
            mqttClient.MqttMsgPublishReceived += (sender, args) =>
            {
                var payload = Encoding.UTF8.GetString(args.Message);
                logger.LogInformation("### RECEIVED APPLICATION MESSAGE ###");
                logger.LogInformation($"+ Topic = {args.Topic}");
                logger.LogInformation($"+ Payload = {payload}");
                logger.LogInformation($"+ QoS = {args.QosLevel}");
                logger.LogInformation($"+ Retain = {args.Retain}");

                var eventContract = JsonConvert.DeserializeObject<EventContract>(payload);
                
                if (!eventContract.Forward)
                {
                    return;
                }
                
                using (var scope = serviceProvider.CreateScope())
                {
                    var outbox = scope.ServiceProvider.GetRequiredService<IBus>();
                    outbox.Advanced.Topics.Publish(EventContract.Topic, eventContract);
                }

            };
            
            mqttClient.Subscribe(MqttTopics, new[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE }); 
            
            return mqttClient;
        }
        
        private static bool Connect(this MqttClient mqttClient, IConfiguration configuration)
        {
            var mqttConfigSection = configuration.GetSection("mqtt");
            mqttClient.Connect(
                mqttConfigSection.GetValue<string>("inputQueue"),
                mqttConfigSection.GetValue<string>("user"), 
                mqttConfigSection.GetValue<string>("password"));

            return mqttClient.IsConnected;
        }
        
        private static bool TryConnect(this MqttClient mqttClient, IConfiguration configuration, ILogger logger)
        {
            try
            {
                mqttClient.Connect(configuration);
                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not connect mqtt client!");
                return false;
            }
        }
    }
}