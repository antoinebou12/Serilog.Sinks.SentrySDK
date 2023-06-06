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
    /// </remarks>
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
        /// <param name="serverName">The name of the service.</param>
        /// <param name="release">The version of your application.</param>
        /// <param name="restrictedToMinimumLevel">The minimum log level for events to pass to the sink.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="tags">Tags to apply to all events.</param>
        /// <param name="transactionName">The name of the transaction.</param>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="sampleRate">The sample rate for events.</param>
        /// <param name="attachStacktrace">Whether to attach a stacktrace to the events or not.</param>
        /// <param name="autoSessionTracking">Whether to automatically track sessions or not.</param>
        /// <param name="enableTracing">Whether the tracing is enabled or not.</param>
        /// <param name="transactionService">The service used for handling transactions.</param>
        /// <param name="tracesSampleRate"> The sample rate for traces.</param>
        /// <param name="stackTraceMode"></param>
        /// 
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
            string serverName = "Sample",
            string release = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
            IFormatProvider formatProvider = null,
            string tags = null,
            string transactionName = null,
            string operationName = null,
            float sampleRate = 1.0f,
            bool attachStacktrace = true,
            bool autoSessionTracking = true,
            bool enableTracing = true,
            ITransactionService transactionService = null,
            double tracesSampleRate = 1.0,
            string stackTraceMode = "Enhanced"
            )
        {
            var options = new SentryOptions
            {
                Dsn = dsn,
                Debug = debug,
                Environment = environment,
                Release = release,
                SendDefaultPii = sendDefaultPii,
                MaxBreadcrumbs = maxBreadcrumbs,
                MaxQueueItems = maxQueueItems,
                DiagnosticLevel = Enum.Parse<SentryLevel>(diagnosticLevel, true),
                SampleRate = (float) sampleRate,
                AttachStacktrace = attachStacktrace,
                AutoSessionTracking = autoSessionTracking,
                EnableTracing = enableTracing,
                TracesSampleRate = (float) tracesSampleRate,
                StackTraceMode = Enum.Parse<StackTraceMode>(stackTraceMode, true)
            };

            var sink = new SentrySink(
                formatProvider,
                new SentrySdkWrapper(options),
                dsn,
                tags,
                includeActivityData,
                sendDefaultPii,
                maxBreadcrumbs,
                maxQueueItems,
                debug,
                diagnosticLevel,
                environment,
                serverName,
                release,
                restrictedToMinimumLevel,
                transactionName,
                operationName,
                sampleRate,
                attachStacktrace,
                autoSessionTracking,
                enableTracing,
                transactionService,
                tracesSampleRate,
                stackTraceMode
            );

            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}
