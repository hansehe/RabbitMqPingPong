using System;
using System.IO;
using System.Reflection;

namespace RabbitMqPingPong.Logging
{
    public static class ApplicationConstants
    {
        public static readonly string LogPath = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(ApplicationConstants)).Location), "log");
        public const string ApplicationName = "RabbitMqPingPong";
    }
}