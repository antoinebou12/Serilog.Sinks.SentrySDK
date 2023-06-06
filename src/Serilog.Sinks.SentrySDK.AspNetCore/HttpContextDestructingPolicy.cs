using System;
using Sentry;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SentrySDK;


namespace Serilog.Sinks.SentrySDK.AspNetCore
{
    public class HttpContextDestructingPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            if (value is ISentryHttpContext)
            {
                result = new ScalarValue(value);
                return true;
            }

            result = null;
            return false;
        }
    }
}
