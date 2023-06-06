using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Sentry;

namespace Serilog.Sinks.SentrySDK
{
    /// <summary>
    /// Provides extension methods to integrate Sentry with Serilog.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To use the Sentry sink, first install the <c>Serilog.Sinks.SentrySDK</c> package from NuGet.
    /// </para>
    public static class SentrySinkExtensions
    {
        /// <summary>
        /// Adds a sink that writes log events to the Sentry service.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="dsn">The DSN of your Sentry project.</param>
        /// <param name="active">Whether the Sentry integration is active or not.</param>
        /// <param name="includeActivityData">Whether to include activity data or not.</param>
        /// <param name="sendDefaultPii">Whether to send default personally identifiable information or not.</param>
        /// <param name="maxBreadcrumbs">The maximum number of breadcrumbs that can be stored in memory.</param>
        /// <param name="maxQueueItems">The maximum number of events that can be stored in the queue.</param>
        /// <param name="debug">Whether the debug mode is enabled or not.</param>
        /// <param name="diagnosticLevel">The minimum log level that will be sent to Sentry.</param>
        /// <param name="environment">The environment name (e.g. production, staging).</param>
        /// <param name="serviceName">The name of the service.</param>
        /// <param name="release">The version of your application.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log level for events to pass to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="tags">Tags to apply to all events.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
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
