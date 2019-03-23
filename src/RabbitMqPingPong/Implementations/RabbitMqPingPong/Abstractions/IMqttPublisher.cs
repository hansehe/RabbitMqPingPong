using System.Threading.Tasks;
using RabbitMqPingPong.Contracts;

namespace RabbitMqPingPong.Abstractions
{
    public interface IMqttPublisher
    {
        Task Publish<T>(T @object);
        Task Publish(string topic, object @object);
    }
}