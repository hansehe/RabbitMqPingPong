using TestUtilities.RabbitMqPingPong;
using Xunit;

namespace Test.RabbitMqPingPong.Tests
{
    [CollectionDefinition(CollectionTag)]
    public class TestCollectionFixture : ICollectionFixture<DatabaseMigrationFixture>
    {
        public const string CollectionTag = "Database migration collection";
    }
}