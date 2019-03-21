using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using RabbitMqPingPong.Contracts;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.SqlBuilder;

namespace RabbitMqPingPong.Database.Select
{
    public static class SelectEvents
    {
        public static Task<IEnumerable<EventContract>> Select(
            IDbConfig dbConfig,
            IDbConnection dbConnection,
            Guid[] ids)
        {
            var sql = dbConfig.BuildSql(ids, out var @params);
            return dbConnection.QueryAsync<EventContract>(sql, @params);
        }
        
        private static string BuildSql(this IDbConfig dbConfig, Guid[] ids, out DynamicParameters @params)
        {
            @params = new DynamicParameters();
            var inSelections = dbConfig.CreateParameterResolver().AddArrayParameter(@params, Columns.Id, ids);
            var sql = dbConfig.CreateSqlBuilder().Select()
                .OnSchema()
                .From<EventContract>(Tables.EventTable)
                .Fields<EventContract>(x => x.F(item => item.Id))
                .Fields<EventContract>(x => x.F(item => item.Event))
                .Fields<EventContract>(x => x.F(item => item.Stop))
                .Where<EventContract>(x => x.WP(item => item.Id, inSelections))
                .Build();
                
            return sql;
        }
    }
}