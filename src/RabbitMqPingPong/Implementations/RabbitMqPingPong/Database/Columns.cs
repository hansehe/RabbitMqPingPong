using RabbitMqPingPong.Contracts;

namespace RabbitMqPingPong.Database
{
    public static class Columns
    {
        public const string Id = nameof(EventContract.Id);
        public const string Event = nameof(EventContract.Event);
        public const string Stop = nameof(EventContract.Stop);
    }
}