using System;
using System.IO;
using System.Reflection;

namespace RabbitMqPingPong.Logging
{
    public static class ApplicationConstants
    {
        public const string ProfilingMatch = "DIPS.Profiling";
        public static readonly string LogPath = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(ApplicationConstants)).Location), "DIPS-log");
        public const int ApplicationId = -100;
        public static readonly Guid SessionId = Guid.NewGuid();
        public const string ApplicationName = "RabbitMqPingPong";
    }
}