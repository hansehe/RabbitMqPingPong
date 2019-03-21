using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using RabbitMqPingPong.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace RabbitMqPingPong.Service.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IDbTransaction DbTransaction;
        private readonly IEventRepository EventRepository;
        private readonly ILogger<SearchController> Logger;

        public SearchController(
            IDbTransaction dbTransaction,
            IEventRepository eventRepository,
            ILogger<SearchController> logger)
        {
            DbTransaction = dbTransaction;
            EventRepository = eventRepository;
            Logger = logger;
        }
        
        [HttpGet("{id}")]
        public async Task<string> SearchEvent(Guid id)
        {
            var events = (await EventRepository.SelectEvents(DbTransaction.Connection, new []{id})).ToList();
            if (events.Any(x => x.Id.Equals(id)))
            {
                var eventContract = events.First(x => x.Id.Equals(id));
                var successMsg = $"Event with id: {id} and message: '{eventContract.Event}' has been received!";
                Logger.LogInformation(successMsg);
                return successMsg;
            }
            var failMessage = $"Event with id: {id} has still not been received..";
            Logger.LogInformation(failMessage);
            return failMessage;
        }
    }
}