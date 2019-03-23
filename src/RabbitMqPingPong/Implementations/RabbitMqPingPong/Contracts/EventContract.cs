using System;

namespace RabbitMqPingPong.Contracts
{
    public class EventContract
    {
        public const string Topic = "Contracts.Event";
        public const string ForwardTopic = "Contracts.Forward.Event";

        public const int MaxEventLength = 100;
        public const int MaxPingPongs = 1000;

        public Guid Id { get; set; } = Guid.NewGuid();
        public string Event { get; set; } = "Some event";
        public bool Stop { get; set; } = false;
        public int PingPongs { get; set; } = 100;
    }
}