using System;
using System.Linq;
using Serilog.Core;
using Serilog.Events;
using Sentry;

namespace Serilog
{
    public class SentrySink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly string[] _tags;
        private readonly SentryOptions _options;
        private readonly IDisposable _sentry;
        public SentrySink(IFormatProvider formatProvider, string dsn, string release, string environment, string tags)
        {
            _formatProvider = formatProvider;
            _options = new SentryOptions
            {
                Dsn = dsn,
                Release = release,
                Environment = environment
            };
            _tags = string.IsNullOrWhiteSpace(tags) ? new string[0] : tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();

            // Initialize the SDK
            _sentry = SentrySdk.Init(_options);
        }

        public void Emit(LogEvent logEvent)
        {
            var exception = logEvent.Exception;
            var message = logEvent.RenderMessage(_formatProvider);
            var level = GetSentryLevel(logEvent);

            SentrySdk.WithScope(scope =>
            {
                scope.Level = level;
                scope.SetExtras(logEvent.Properties.Where(pair => _tags.All(t => t != pair.Key))
                    .ToDictionary(pair => pair.Key, pair => Render(pair.Value, _formatProvider)));
                scope.SetTags(logEvent.Properties.Where(pair => _tags.Any(t => t == pair.Key))
                    .ToDictionary(pair => pair.Key, pair => Render(pair.Value, _formatProvider)));


                if (exception == null)
                {
                    SentrySdk.CaptureMessage(message, level);
                }
                else
                {
                    SentrySdk.CaptureException(exception);
                }
            });
        }

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
