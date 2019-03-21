using System.Threading.Tasks;
using RabbitMqPingPong.Contracts;
using Rebus.Bus;

namespace RabbitMqPingPong
{
    public static class SubscriptionRegistration
    {
        public static async Task SetupSubscriptions(IBus bus)
        {
            await bus.Advanced.Topics.Subscribe(EventContract.Topic);
        }
    }
}