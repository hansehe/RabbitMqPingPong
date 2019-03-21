using System;
using Microsoft.Extensions.Logging;
using Rebus.Logging;

namespace RabbitMqPingPong.LoggerAdapter
{
    internal class MsRebusLogger : ILog
    {
        private readonly Func<string, object[], string> RenderStringFunc;
        
        private readonly ILogger Logger;

        public MsRebusLogger(ILogger logger,
            Func<string, object[], string> renderStringFunc)
        {
            Logger = logger;
            RenderStringFunc = renderStringFunc;
        }
        
        public void Debug(string message, params object[] objs)
        {
            Logger.LogDebug(RenderStringFunc.Invoke(message, objs));
        }

        public void Info(string message, params object[] objs)
        {
            Logger.LogInformation(RenderStringFunc.Invoke(message, objs));
        }

        public void Warn(string message, params object[] objs)
        {
            Logger.LogWarning(RenderStringFunc.Invoke(message, objs));
        }

        public void Warn(Exception exception, string message, params object[] objs)
        {
            Logger.LogWarning(exception, RenderStringFunc.Invoke(message, objs));
        }

        public void Error(string message, params object[] objs)
        {
            Logger.LogError(RenderStringFunc.Invoke(message, objs));
        }

        public void Error(Exception exception, string message, params object[] objs)
        {
            Logger.LogError(exception, RenderStringFunc.Invoke(message, objs));
        }
    }
}