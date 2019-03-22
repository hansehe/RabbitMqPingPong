using System.Text;
using System.Threading.Tasks;
using RabbitMqPingPong.Abstractions;
using RabbitMqPingPong.Contracts;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace RabbitMqPingPong.Mqtt
{
    public class MqttPublisher : IMqttPublisher
    {
        private readonly MqttClient MqttClient;

        public MqttPublisher(MqttClient mqttClient)
        {
            MqttClient = mqttClient;
        }
        
        public Task Publish(EventContract eventContract)
        {
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventContract));

            MqttClient.Publish(EventContract.Topic, payload, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
            
            return Task.CompletedTask;
        }
    }
}