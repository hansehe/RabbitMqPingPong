using System.Data;
using System.Threading.Tasks;
using RabbitMqPingPong;
using RabbitMqPingPong.Abstractions;
using RabbitMqPingPong.Contracts;
using RabbitMqPingPong.Mqtt;
using FluentAssertions;
using FluentDbTools.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using TestUtilities.RabbitMqPingPong;
using uPLibrary.Networking.M2Mqtt;
using Xunit;

namespace Test.RabbitMqPingPong.Tests
{
    [Collection(TestCollectionFixture.CollectionTag)]
    public class MessageHandlerTests
    {
        [Theory]
        [MemberData(nameof(TestParameters.DbParameters), MemberType = typeof(TestParameters))]
        public async Task PublishSubscribe_Event_Success(SupportedDatabaseTypes databaseType)
        {
            var scope = OverrideConfig.GetServiceCollection(databaseType)
                .BuildServiceProvider()
                .CreateScope();
            
            var mqttClient = scope.ServiceProvider.GetRequiredService<MqttClient>();
            var bus = scope.ServiceProvider.GetRequiredService<IBus>();
            var repository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
            var dbConnection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
            
            await SubscriptionRegistration.SetupSubscriptions(bus);
            mqttClient.Setup(scope.ServiceProvider);
            
            var eventContract = new EventContract();
            eventContract.PingPongs = 5;
            await bus.Advanced.Topics.Publish(EventContract.Topic, eventContract);

            await Tools.WaitUntilSuccess(async () =>
            {
                var selectedEvent = await repository.SelectEvent(dbConnection, eventContract.Id); // At some point, the event should have been sent through the message broker and handled by the message handler.
                selectedEvent.Id.Should().Be(eventContract.Id); // Verify that the message handler has received and handled the event for the first time. The message handler will publish the event again, but having set the Stop event property to true.
                selectedEvent.Stop.Should().BeTrue(); // Verify that the message handler published and handled the event again by marking the event as stopped.
            });

            await repository.DeleteEvent(dbConnection, eventContract.Id); // Finished with the event, delete it.
        }
    }
}