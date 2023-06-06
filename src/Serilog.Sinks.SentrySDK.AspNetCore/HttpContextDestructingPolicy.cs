using System;
using Sentry;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SentrySDK;

namespace Serilog.Sinks.SentrySDK.AspNetCore
{
    /// <summary>
    /// Destructuring policy for Serilog to handle <see cref="ISentryHttpContext"/> instances.
    /// </summary>
    public class HttpContextDestructingPolicy : IDestructuringPolicy
    {
        /// <summary>
        /// Tries to destructure the provided value.
        /// </summary>
        /// <param name="value">The value to destructure.</param>
        /// <param name="propertyValueFactory">The property value factory.</param>
        /// <param name="result">The destructure result.</param>
        /// <returns><c>true</c> if the value was destructured successfully; otherwise, <c>false</c>.</returns>
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
