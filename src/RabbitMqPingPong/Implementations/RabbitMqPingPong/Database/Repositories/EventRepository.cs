using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using RabbitMqPingPong.Abstractions;
using RabbitMqPingPong.Contracts;
using FluentDbTools.Common.Abstractions;

namespace RabbitMqPingPong.Database.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbConfig DbConfig;

        public EventRepository(IDbConfig dbConfig)
        {
            DbConfig = dbConfig;
        }
        
        public Task InsertEvent(IDbConnection dbConnection, EventContract eventContract)
        {
            return Insert.InsertEvent.Execute(
                DbConfig,
                dbConnection,
                eventContract);
        }

        public async Task<EventContract> SelectEvent(IDbConnection dbConnection, Guid id)
        {
            return (await SelectEvents(
                dbConnection,
                new []{id})).First();
        }

        public Task<IEnumerable<EventContract>> SelectEvents(IDbConnection dbConnection, IEnumerable<Guid> ids)
        {
            return Select.SelectEvents.Select(
                DbConfig,
                dbConnection,
                ids.ToArray());
        }

        public Task UpdateEvent(IDbConnection dbConnection, EventContract eventContract)
        {
            return Update.UpdateEvent.Execute(
                DbConfig,
                dbConnection,
                eventContract);
        }

        public Task DeleteEvent(IDbConnection dbConnection, Guid id)
        {
            return Delete.DeleteEvent.Execute(
                DbConfig,
                dbConnection,
                id);
        }
    }
}