using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Sinks.SentrySDK.AspNetCore
{
    public static class SerilogSentryExtensions
    {
        public static LoggerConfiguration Sentry(
            this LoggerSinkConfiguration loggerConfiguration,
            string dsn,
            bool active,
            bool includeActivityData,
            bool sendDefaultPii,
            int maxBreadcrumbs,
            int maxQueueItems,
            bool debug,
            string diagnosticLevel,
            string environment,
            string serviceName,
            string release,
            LogEventLevel minimumLevel = LogEventLevel.Error)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (dsn == null) throw new ArgumentNullException(nameof(dsn));

            var options = new SentryOptions
            {
                Dsn = dsn,
                IsActive = active,
                IncludeActivityData = includeActivityData,
                SendDefaultPii = sendDefaultPii,
                MaxBreadcrumbs = maxBreadcrumbs,
                MaxQueueItems = maxQueueItems,
                Debug = debug,
                DiagnosticLevel = diagnosticLevel,
                Environment = environment,
                ServiceName = serviceName,
                Release = release
            };

            return loggerConfiguration.Sink(
                new SentrySink(options, formatProvider: null),
                restrictedToMinimumLevel: minimumLevel);
        }
    }
}
