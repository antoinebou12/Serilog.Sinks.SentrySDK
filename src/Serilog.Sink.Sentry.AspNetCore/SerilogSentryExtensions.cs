using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace YourNamespace
{
    public static class SerilogSentryExtensions
    {
        /// <summary>
        /// Adds a sink that sends log events to the Sentry.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="dsn">The Sentry DSN.</param>
        /// <param name="minimumLevel">The minimum log event level required 
        /// in order to write an event to the sink.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Sentry(
            this LoggerSinkConfiguration loggerConfiguration,
            string dsn,
            LogEventLevel minimumLevel = LogEventLevel.Error)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (dsn == null) throw new ArgumentNullException(nameof(dsn));

            return loggerConfiguration.Sink(
                new SentrySink(dsn, formatProvider: null),
                restrictedToMinimumLevel: minimumLevel);
        }
    }
}
