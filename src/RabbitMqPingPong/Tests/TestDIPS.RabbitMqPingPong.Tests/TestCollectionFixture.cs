using TestUtilities.RabbitMqPingPong;
using Xunit;

namespace TestDIPS.RabbitMqPingPong.Tests
{
    [CollectionDefinition(CollectionTag)]
    public class TestCollectionFixture : ICollectionFixture<DatabaseMigrationFixture>
    {
        public const string CollectionTag = "Database migration collection";
    }
}