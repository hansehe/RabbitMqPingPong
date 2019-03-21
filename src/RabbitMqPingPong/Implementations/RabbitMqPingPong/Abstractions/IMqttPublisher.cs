using System.Threading.Tasks;
using RabbitMqPingPong.Contracts;

namespace RabbitMqPingPong.Abstractions
{
    public interface IMqttPublisher
    {
        Task Publish(EventContract eventContract);
    }
}