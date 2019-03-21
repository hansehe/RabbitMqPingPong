using System;
using System.Runtime.InteropServices;

namespace RabbitMqPingPong.Config
{
    public static class BaseConfig
    {
        private const string ConfigFolder = "DefaultConfigs";
        
        public static readonly string DefaultConfigFilename = $"{ConfigFolder}/DefaultConfig.json";
        public static readonly string DefaultConfigDockerFilename = $"{ConfigFolder}/DefaultConfig.Docker.json";
        
        public static bool InContainer => 
            Environment.GetEnvironmentVariable("RUNNING_IN_CONTAINER") == "true";

        public static void UpdateCaCertificates()
        {
            if (!InContainer || !RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return;
            }
            
            const string cmd = "update-ca-certificates";
            ShellHelper.Bash(cmd);
        }
    }
}