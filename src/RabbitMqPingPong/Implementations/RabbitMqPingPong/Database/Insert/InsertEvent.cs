using System.Data;
using System.Threading.Tasks;
using Dapper;
using RabbitMqPingPong.Contracts;
using FluentDbTools.Common.Abstractions;
using FluentDbTools.Extensions.SqlBuilder;

namespace RabbitMqPingPong.Database.Insert
{
    public static class InsertEvent
    {
        public static Task Execute(
            IDbConfig dbConfig,
            IDbConnection dbConnection,
            EventContract eventContract)
        {
            var @params = new DynamicParameters();
            @params.Add(Columns.Id, dbConfig.CreateParameterResolver().WithGuidParameterValue(eventContract.Id));
            @params.Add(Columns.Event, eventContract.Event);
            @params.Add(Columns.Stop, dbConfig.CreateParameterResolver().WithBooleanParameterValue(eventContract.Stop));
            var sql = dbConfig.BuildSql();
            return dbConnection.ExecuteAsync(sql, @params);
        }
        
        private static string BuildSql(this IDbConfig dbConfig)
        {
            var sql = dbConfig.CreateSqlBuilder().Insert<EventContract>()
                .OnTable(Tables.EventTable)
                .OnSchema()
                .Fields(x => x.FP(item => item.Id))
                .Fields(x => x.FP(item => item.Event))
                .Fields(x => x.FP(item => item.Stop))
                .Build();
            return sql;
        }
    }
}