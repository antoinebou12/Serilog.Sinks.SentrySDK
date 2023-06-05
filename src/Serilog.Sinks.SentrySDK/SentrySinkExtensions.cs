using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Sentry;

namespace Serilog.Sinks.SentrySDK
{
    public static class SentrySinkExtensions
    {
        public static LoggerConfiguration Sentry(
            this LoggerSinkConfiguration loggerConfiguration,
            string dsn,
            bool active = true,
            bool includeActivityData = true,
            bool sendDefaultPii = true,
            int maxBreadcrumbs = 200,
            int maxQueueItems = 100,
            bool debug = true,
            string diagnosticLevel = "Error",
            string environment = null,
            string serviceName = "Sample",
            string release = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
            IFormatProvider formatProvider = null,
            string tags = null)
        {
            var options = new SentryOptions
            {
                Dsn = dsn,
                Debug = debug,
                Environment = environment,
                Release = release
            };

            var sink = new SentrySink(formatProvider, options, tags);
            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}
