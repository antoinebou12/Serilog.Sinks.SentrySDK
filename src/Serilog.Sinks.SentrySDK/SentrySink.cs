using System;
using System.Linq;
using Serilog.Core;
using Serilog.Events;
using Sentry;

namespace Serilog.Sinks.SentrySDK
{
    /// <summary>
    /// Provides a sink that directs log events to the Sentry service.
    /// </summary>
    public class SentrySink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly string[] _tags;
        private readonly SentryOptions _options;
        private readonly IDisposable _sentry;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentrySink"/> class.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="options">The Sentry options.</param>
        /// <param name="tags">The tags.</param>
        public SentrySink(IFormatProvider formatProvider, SentryOptions options, string tags)
        {
            _formatProvider = formatProvider;
            _options = options;
            _tags = string.IsNullOrWhiteSpace(tags) ? new string[0] : tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();

            // Initialize the SDK
            _sentry = SentrySdk.Init(_options);
        }

        /// <summary>
        /// Emit the log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to emit.</param>
        public void Emit(LogEvent logEvent)
        {
            var level = GetSentryLevel(logEvent);
            var transaction = SentrySdk.StartTransaction(
                name: "transaction name",
                operation: "operation name"
            );

            try
            {
                SentrySdk.WithScope(scope =>
                {
                    scope.Level = level;
                    scope.SetExtras(logEvent.Properties.Where(pair => _tags.All(t => t != pair.Key))
                        .ToDictionary(pair => pair.Key, pair => (object)Render(pair.Value, _formatProvider)));
                    scope.SetTags(
                        logEvent.Properties.Where(pair => _tags.Any(t => t == pair.Key))
                            .ToDictionary(pair => pair.Key, pair => Render(pair.Value, _formatProvider)));

                    var span = transaction.StartChild("operation name");
                    try
                    {
                        if (logEvent.Exception == null)
                        {
                            SentrySdk.CaptureMessage(logEvent.RenderMessage(_formatProvider), level);
                        }
                        else
                        {
                            SentrySdk.CaptureException(logEvent.Exception);
                        }
                    }
                    catch (Exception)
                    {
                        // If an error occurs, set the status of the span to failed.
                        span.Status = SpanStatus.InternalError;
                        throw;
                    }
                    finally
                    {
                        // Regardless of outcome, finish the span.
                        span.Finish();
                    }
                });
            }
            finally
            {
                // Regardless of outcome, finish the transaction.
                transaction.Finish();
            }
        }

        /// <summary>
        /// Maps a Serilog event level to a Sentry level.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>The Sentry level.</returns>
        private static SentryLevel GetSentryLevel(LogEvent logEvent)
        {
            return logEvent.Level switch
            {
                LogEventLevel.Verbose => SentryLevel.Debug,
                LogEventLevel.Debug => SentryLevel.Debug,
                LogEventLevel.Information => SentryLevel.Info,
                LogEventLevel.Warning => SentryLevel.Warning,
                LogEventLevel.Error => SentryLevel.Error,
                LogEventLevel.Fatal => SentryLevel.Fatal,
                _ => SentryLevel.Error
            };
        }

        /// <summary>
        /// Renders the log event property value as a string.
        /// </summary>
        /// <param name="logEventPropertyValue">The log event property value.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A string representation of the log event property value.</returns>
        private static string Render(LogEventPropertyValue logEventPropertyValue, IFormatProvider formatProvider)
        {
            if (logEventPropertyValue is ScalarValue scalarValue && scalarValue.Value is string stringValue)
            {
                // Remove quotes from the value
                return stringValue;
            }

            return logEventPropertyValue != null ? logEventPropertyValue.ToString(null, formatProvider) : null;
        }
    }
}
