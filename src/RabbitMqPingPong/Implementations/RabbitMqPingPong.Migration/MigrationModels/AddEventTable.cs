using RabbitMqPingPong.Database;
using FluentDbTools.Migration.Contracts;
using FluentMigrator;

namespace RabbitMqPingPong.Migration.MigrationModels
{
    [Migration(0)]
    public class AddEventTable : MigrationModel
    {
        public override void Up()
        {
            Create.Table(Tables.EventTable)
                .InSchema(SchemaName)
                .WithColumn(Columns.Id).AsGuid().PrimaryKey()
                .WithColumn(Columns.Event).AsString()
                .WithColumn(Columns.Stop).AsBoolean();
        }

        public override void Down()
        {
            Delete.Table(Tables.EventTable)
                .InSchema(SchemaName);
        }
    }
}