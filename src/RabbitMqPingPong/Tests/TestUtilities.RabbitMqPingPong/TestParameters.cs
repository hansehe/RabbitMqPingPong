﻿using System.Collections.Generic;
using FluentDbTools.Common.Abstractions;

namespace TestUtilities.RabbitMqPingPong
{
    public static class TestParameters
    {
        public static IEnumerable<object[]> DbParameters =>
            new List<object[]>
            {                
                new object[] { SupportedDatabaseTypes.Postgres }
            };
    }
}