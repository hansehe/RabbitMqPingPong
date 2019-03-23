using System.Data;
using System.Threading.Tasks;
using RabbitMqPingPong.Abstractions;
using RabbitMqPingPong.Contracts;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;

namespace RabbitMqPingPong.MessageHandlers
{
    public class EventMessageHandler : IHandleMessages<EventContract>
    {
        private readonly ILogger<EventMessageHandler> Logger;
        private readonly IEventRepository EventRepository;
        private readonly IDbTransaction DbTransaction;
        private readonly IBus Bus;
        private readonly IMqttPublisher MqttPublisher;

        public EventMessageHandler(
            ILogger<EventMessageHandler> logger,
            IEventRepository eventRepository,
            IDbTransaction dbTransaction,
            IBus bus,
            IMqttPublisher mqttPublisher)
        {
            Logger = logger;
            EventRepository = eventRepository;
            DbTransaction = dbTransaction;
            Bus = bus;
            MqttPublisher = mqttPublisher;
        }
        
        public async Task Handle(EventContract message)
        {
            Logger.LogInformation($"Received contract with event: {message.Event}");
            message.PingPongs--;
            
            if (message.Stop)
            {
                Logger.LogInformation("Received final stop of the event");
                await EventRepository.UpdateEvent(DbTransaction.Connection, message);
                DbTransaction.Commit();
                return;
            }
            
            if (message.Event.Length > EventContract.MaxEventLength)
            {
                message.Event = message.Event.Substring(0, EventContract.MaxEventLength);
            }

            const int maxPingPongs = EventContract.MaxPingPongs;
            if (message.PingPongs > maxPingPongs)
            {
                message.PingPongs = EventContract.MaxPingPongs;
            }
            
            await MqttPublisher.Publish(EventContract.Topic, message);
            
            Logger.LogInformation("Publishing event again with reply command");

            if (message.PingPongs <= 0)
            {
                await EventRepository.InsertEvent(DbTransaction.Connection, message);
                message.Stop = true;
            }
            
            // A reply returns the response directly back to the queue owned by the originating publisher of the event.
            await Bus.Reply(message);
            DbTransaction.Commit();
        }
    }
}