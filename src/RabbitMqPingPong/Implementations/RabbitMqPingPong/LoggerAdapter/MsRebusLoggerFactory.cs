using System;
using Microsoft.Extensions.Logging;
using Rebus.Logging;

namespace RabbitMqPingPong.LoggerAdapter
{
    internal class MsRebusLoggerFactory : AbstractRebusLoggerFactory
    {
        private readonly ILoggerFactory LoggerFactory;

        public MsRebusLoggerFactory(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }

        protected override ILog GetLogger(Type type)
        {
            var logger = LoggerFactory.CreateLogger(type);
            return new MsRebusLogger(logger, RenderString);
        }
    }
}