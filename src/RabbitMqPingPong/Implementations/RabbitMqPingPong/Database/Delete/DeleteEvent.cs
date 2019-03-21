using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using RabbitMqPingPong.Contracts;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.SqlBuilder;

namespace RabbitMqPingPong.Database.Delete
{
    public static class DeleteEvent
    {
        public static Task Execute(
            IDbConfig dbConfig,
            IDbConnection dbConnection,
            Guid id)
        {
            var @params = new DynamicParameters();
            @params.Add(Columns.Id, dbConfig.CreateParameterResolver().WithGuidParameterValue(id));
            var sql = dbConfig.BuildSql();
            return dbConnection.ExecuteAsync(sql, @params);
        }
        
        private static string BuildSql(this IDbConfig dbConfig)
        {
            var sql = dbConfig.CreateSqlBuilder().Delete<EventContract>()
                .OnTable(Tables.EventTable)
                .OnSchema()
                .Where(x => x.WP(item => item.Id))
                .Build();
            return sql;
        }
    }
}