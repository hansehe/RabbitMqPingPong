using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using RabbitMqPingPong.Contracts;
using FluentDbTools.Common.Abstractions;

namespace RabbitMqPingPong.Abstractions
{
    public interface IEventRepository
    {
        Task InsertEvent(IDbConnection dbConnection, EventContract eventContract);

        Task<EventContract> SelectEvent(IDbConnection dbConnection, Guid id);

        Task<IEnumerable<EventContract>> SelectEvents(IDbConnection dbConnection, IEnumerable<Guid> ids);
        
        Task UpdateEvent(IDbConnection dbConnection, EventContract eventContract);
        
        Task DeleteEvent(IDbConnection dbConnection, Guid id);
    }
}