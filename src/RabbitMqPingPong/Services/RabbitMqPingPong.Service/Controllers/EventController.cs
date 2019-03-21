using System.Data;
using System.Threading.Tasks;
using RabbitMqPingPong.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Transport;

namespace RabbitMqPingPong.Service.Controllers
{
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IBus Bus;
        private readonly ILogger<EventController> Logger;

        public EventController(
            IBus bus,
            ILogger<EventController> logger)
        {
            Bus = bus;
            Logger = logger;
        }
        
        [HttpGet]
        public async Task<string> TriggerEvent()
        {
            var eventContract = new EventContract();
            await TriggerEvent(eventContract);
            var msg = $"Triggered new event with id: {eventContract.Id}";
            Logger.LogInformation(msg);
            return msg;
        }
        
        [HttpGet("{eventMessage}")]
        public async Task<string> TriggerEvent(string eventMessage)
        {
            var eventContract = new EventContract {Event = eventMessage};
            await TriggerEvent(eventContract);
            var msg = $"Triggered new event with id: {eventContract.Id} and message: '{eventContract.Event}'";
            Logger.LogInformation(msg);
            return msg;
        }
        
        [HttpPost]
        public async Task TriggerEvent([FromBody]EventContract eventContract)
        {
            Logger.LogInformation($"Storing event with id {eventContract.Id.ToString()} in outbox for publishing.");

            using (var rebusTransactionScope = new RebusTransactionScope())
            {
                await Bus.Advanced.Topics.Publish(EventContract.Topic, eventContract);
                rebusTransactionScope.Complete();
            }
            
            Logger.LogInformation($"Event with id {eventContract.Id.ToString()} successfully stored in outbox.");
        }
    }
}