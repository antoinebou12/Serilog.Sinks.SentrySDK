#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Collections;

using Sentry;


using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.SentrySDK
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SentrySink"/> class.
    /// </summary>
    public interface ISentrySdkWrapper
    {
        /// <summary>
        /// Captures a message to Sentry.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The level of the message.</param>
        void CaptureMessage(string message, SentryLevel level);
        /// <summary>
        /// Captures an exception to Sentry.
        /// </summary>
        /// <param name="exception">The exception to capture.</param>
        void CaptureException(Exception exception);
        /// <summary>
        /// Starts a transaction.
        /// </summary>
        /// <param name="name">The name of the transaction.</param>
        /// <param name="operation">The operation of the transaction.</param>
        ITransactionTracer StartTransaction(string name, string operation);
    }

    /// <summary>
    /// SentrySdkWrapper
    /// </summary>
    /// <seealso cref="Serilog.Sinks.SentrySDK.ISentrySdkWrapper" />
    public class SentrySdkWrapper : ISentrySdkWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SentrySdkWrapper"/> class.
        /// Initialization is performed by <see cref="SentrySink"/> via a single <see cref="SentrySdk.Init(SentryOptions)"/> call.
        /// </summary>
        public SentrySdkWrapper()
        {
        }
        /// <inheritdoc/>
        public void CaptureMessage(string message, SentryLevel level)
        {
            SentrySdk.CaptureMessage(message, level);
        }

        /// <inheritdoc/>
        public void CaptureException(Exception exception)
        {
            SentrySdk.CaptureException(exception);
        }

        /// <inheritdoc/>
        public ITransactionTracer StartTransaction(string name, string operation)
        {
            return SentrySdk.StartTransaction(name, operation);
        }
    }

    /// <summary>
    /// TransactionService
    /// </summary>
    /// <seealso cref="Serilog.Sinks.SentrySDK.ITransactionService" />
    public interface ITransactionService
    {
        /// <summary>
        /// /// Starts the child.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="description"></param>
        /// <returns></returns>
        ISpan StartChild(ITransactionTracer transaction, string operation, string description);
    }

    /// <summary>
    /// TransactionService
    /// </summary>
    /// <seealso cref="Serilog.Sinks.SentrySDK.ITransactionService" />
    public class TransactionService : ITransactionService
    {
        /// <inheritdoc/>
        public ISpan StartChild(ITransactionTracer transaction, string operation, string description)
        {
            return transaction.StartChild(operation, description);
        }
    }


    /// <summary>
    /// Provides a sink that directs log events to the Sentry service.
    /// </summary>
    public class SentrySink : ILogEventSink, IDisposable
    {
        private readonly IFormatProvider? _formatProvider;
        private readonly string[] _tags;
        private readonly SentryOptions _options;
        private readonly IDisposable? _sentry;
        private readonly string _transactionName;
        private readonly string _operationName;
        private readonly ISentrySdkWrapper _sentrySdkWrapper;
        private readonly ITransactionService _transactionService;
        private readonly string? _restrictedToMinimumLevel;
        private readonly bool _enableTracing;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentrySink"/> class.
        /// </summary>
        /// <param name="formatProvider">The format provider, or null for default formatting.</param>
        /// <param name="sentrySdkWrapper"> sentry sdk wrapper</param>
        /// <param name="dsn"> sentry dsn</param>
        /// <param name="tags">The tags, or null.</param>
        /// <param name="sendDefaultPii"> send default pii</param>
        /// <param name="maxBreadcrumbs"> max breadcrumbs</param>
        /// <param name="maxQueueItems"> max queue items</param>
        /// <param name="debug"> debug</param>
        /// <param name="diagnosticLevel"> diagnostic level</param>
        /// <param name="environment"> environment, or null.</param>
        /// <param name="serverName"> server name, or null.</param>
        /// <param name="release"> release, or null.</param>
        /// <param name="restrictedToMinimumLevel"> restricted to minimum level, or null.</param>
        /// <param name="transactionName"> transaction name, or null.</param>
        /// <param name="operationName"> operation name, or null.</param>
        /// <param name="sampleRate"> sample rate</param>
        /// <param name="attachStacktrace"> attach stacktrace</param>
        /// <param name="autoSessionTracking"> auto session tracking</param>
        /// <param name="enableTracing"> enable tracing</param>
        /// <param name="transactionService"> transaction service, or null to use <see cref="TransactionService"/>.</param>
        /// <param name="tracesSampleRate"> traces sample rate</param>
        /// <param name="stackTraceMode"> stack trace mode</param>
        /// <param name="isEnvironmentUser"> is environment user</param>
        /// <param name="shutdownTimeout"> shutdown timeout</param>
        /// <param name="maxCacheItems"> max cache items</param>
        /// <param name="distribution"> distribution, or null.</param>
        /// <param name="configureSentryOptions">Optional callback for advanced <see cref="SentryOptions"/> (for example metrics, structured logs). Invoked before <c>SetBeforeSend</c>; avoid calling <c>SetBeforeSend</c> here because the sink registers a chained callback—use <paramref name="beforeSend"/> instead.</param>
        /// <param name="beforeSend">Optional callback after the sink's EventId mapping. Return <c>null</c> to discard the event.</param>
        public SentrySink(
            IFormatProvider? formatProvider,
            ISentrySdkWrapper sentrySdkWrapper,
            string dsn,
            string? tags,
            bool sendDefaultPii,
            int maxBreadcrumbs,
            int maxQueueItems,
            bool debug,
            string diagnosticLevel,
            string? environment,
            string? serverName,
            string? release,
            string? restrictedToMinimumLevel,
            string? transactionName,
            string? operationName,
            float sampleRate,
            bool attachStacktrace,
            bool autoSessionTracking,
            bool enableTracing,
            ITransactionService? transactionService,
            double tracesSampleRate, string stackTraceMode,
            bool isEnvironmentUser,
            double shutdownTimeout,
            int maxCacheItems,
            string? distribution,
            Action<SentryOptions>? configureSentryOptions = null,
            Func<SentryEvent, SentryHint, SentryEvent?>? beforeSend = null
        )
        {
            _formatProvider = formatProvider;
            _sentrySdkWrapper = sentrySdkWrapper;
            _tags = string.IsNullOrWhiteSpace(tags) ? new string[0] : tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
            _transactionName = transactionName ?? string.Empty;
            _operationName = operationName ?? string.Empty;
            _restrictedToMinimumLevel = restrictedToMinimumLevel;
            if (string.IsNullOrWhiteSpace(dsn))
            {
                //Verbose error message to help with configuration
                throw new ArgumentNullException(nameof(dsn), "A DSN must be provided to the Sentry sink (Client Keys DSN). See https://docs.sentry.io/error-reporting/configuration/?platform=csharp for more information.");
            }
            if (transactionService == null)
            {
                _transactionService = new TransactionService();
            }
            else
            {
                _transactionService = transactionService;
            }

            _enableTracing = enableTracing;

            _options = new SentryOptions
            {
                Dsn = dsn,
                SendDefaultPii = sendDefaultPii,
                MaxBreadcrumbs = maxBreadcrumbs,
                MaxQueueItems = maxQueueItems,
                Debug = debug,
                DiagnosticLevel = Enum.Parse<SentryLevel>(diagnosticLevel, true),
                Environment = environment,
                Release = release,
                SampleRate = sampleRate,
                AttachStacktrace = attachStacktrace,
                AutoSessionTracking = autoSessionTracking,
                ServerName = serverName ?? Environment.MachineName,
                TracesSampleRate = enableTracing ? tracesSampleRate : 0.0,
                StackTraceMode = Enum.Parse<StackTraceMode>(stackTraceMode, true),
                IsEnvironmentUser = isEnvironmentUser,
                ShutdownTimeout = TimeSpan.FromSeconds(shutdownTimeout),
                Distribution = distribution,
            };
            if (maxCacheItems > 0)
            {
                _options.MaxCacheItems = maxCacheItems;
            }

            configureSentryOptions?.Invoke(_options);
            if (!enableTracing)
            {
                _options.TracesSampleRate = 0.0;
            }

            _options.SetBeforeSend((sentryEvent, hint) =>
            {
                SentryEvent? processed = sentryEvent;
                var exception = processed?.Exception;
                if (exception != null && exception.Data.Contains("EventId"))
                {
                    var newEvent = new SentryEvent(exception)
                    {
                        Level = processed!.Level // Also copy the Level from the original event
                    };

                    foreach (DictionaryEntry entry in exception.Data)
                    {
                        var key = entry.Key?.ToString();
                        if (string.IsNullOrEmpty(key))
                        {
                            continue;
                        }

                        if (key == "EventId")
                        {
                            newEvent.SetExtra(key, exception.Data["EventId"]?.ToString() ?? string.Empty);
                        }
                        else
                        {
                            newEvent.SetExtra(key, entry.Value?.ToString() ?? string.Empty);
                        }
                    }
                    processed = newEvent;
                }

                if (beforeSend != null)
                {
                    return processed is null ? null : beforeSend(processed, hint);
                }

                return processed;
            });
            IDisposable? sentryDisposable = null;
            try
            {
                // Initialize the SDK
                sentryDisposable = SentrySdk.Init(_options);

                SentrySdk.ConfigureScope(scope =>
                {

                    // Add the server name to the scope
                    if (!string.IsNullOrWhiteSpace(serverName))
                    {
                        scope.SetTag("server", serverName);
                    }

                    // Add the tags to the scope
                    if (_tags.Length > 0)
                    {
                        scope.SetTags(_tags.Select(t => t.Split('='))
                            .Where(t => t.Length == 2)
                            .ToDictionary(t => t[0], t => t[1]));

                    }

                    // Add the transaction name to the scope
                    if (!string.IsNullOrWhiteSpace(_transactionName))
                    {
                        scope.SetTag("transaction", _transactionName);
                    }

                    // Add the operation name to the scope
                    if (!string.IsNullOrWhiteSpace(_operationName))
                    {
                        scope.SetTag("operation", _operationName);
                    }
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize Sentry SDK: {ex.Message}");
            }

            _sentry = sentryDisposable;

        }

        /// <summary>
        /// Emit the log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to emit.</param>
        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                return;
            }
            // minimum level check happens before calling this method restrictedToMinimumLevel is string
            if (_restrictedToMinimumLevel != null && logEvent.Level < (LogEventLevel)Enum.Parse(typeof(LogEventLevel), _restrictedToMinimumLevel ?? "Information", true))
            {
                return;
            }
            var level = GetSentryLevel(logEvent);
            if (!_enableTracing)
            {
                EmitWithoutTracing(logEvent, level);
                return;
            }

            var transaction = _sentrySdkWrapper.StartTransaction(
                name: _transactionName + " " + logEvent.MessageTemplate.Text,
                operation: _operationName + " " + logEvent.MessageTemplate.Text
            );

            try
            {
                using (SentrySdk.PushScope())
                {
                    SentrySdk.ConfigureScope(scope =>
                    {
                        scope.Transaction = transaction;
                        scope.Level = level;
                        scope.SetExtras(logEvent.Properties.Where(pair => _tags.All(t => t != pair.Key))
                            .ToDictionary(pair => pair.Key, pair => (object?)(Render(pair.Value, _formatProvider) ?? string.Empty)));
                        scope.SetTags(
                            RenderPropertyValuesToStringDictionary(
                                logEvent.Properties.Where(pair => _tags.Any(t => t == pair.Key)),
                                _formatProvider));

                        if (_tags.Length > 0)
                        {
                            scope.SetTags(_tags.Select(t => t.Split('='))
                                .Where(t => t.Length == 2)
                                .ToDictionary(t => t[0], t => t[1]));
                        }

                        scope.SetExtras((logEvent.Exception?.Data ?? new Dictionary<string, object>())
                        .Cast<KeyValuePair<string, object?>>());


                        scope.SetExtras(logEvent.MessageTemplate.Tokens
                            .OfType<PropertyToken>()
                            .Select(pt => new KeyValuePair<string, object?>(pt.PropertyName, logEvent.Properties[pt.PropertyName]))
                            .Where(pair => _tags.All(t => t != pair.Key))
                            .ToDictionary(pair => pair.Key, pair => pair.Value));

                        var span = _transactionService.StartChild(
                            transaction: transaction,
                            operation: _operationName,
                            description: logEvent.RenderMessage(_formatProvider)
                        );

                        scope.SetFingerprint($"{logEvent.MessageTemplate.Text} {span.SpanId}");
                        scope.AddBreadcrumb(
                            new Breadcrumb(
                                message: logEvent.MessageTemplate.Text,
                                type: "log",
                                data: RenderPropertyValuesToStringDictionary(logEvent.Properties, _formatProvider),
                                category: logEvent.Level.ToString(),
                                level: GetBreadcrumbLevel(logEvent)
                            )
                        );

                        try
                        {
                            // Logging
                            if (logEvent.Exception == null)
                            {
                                _sentrySdkWrapper.CaptureMessage(logEvent.MessageTemplate.Text, level);
                                scope.AddBreadcrumb(
                                    new Breadcrumb(
                                        message: logEvent.MessageTemplate.Text,
                                        type: "log",
                                        data: RenderPropertyValuesToStringDictionary(logEvent.Properties, _formatProvider),
                                        category: logEvent.Level.ToString(),
                                        level: GetBreadcrumbLevel(logEvent)
                                    )
                                );

                            }
                            // Exception
                            else
                            {
                                _sentrySdkWrapper.CaptureException(logEvent.Exception);
                                scope.SetTag("exception_type", logEvent.Exception.GetType().Name.ToString());
                                scope.AddBreadcrumb(
                                    new Breadcrumb(
                                        message: logEvent.Exception.Message,
                                        type: "exception",
                                        data: ExceptionDataToStringDictionary(logEvent.Exception.Data),
                                        category: logEvent.Level.ToString(),
                                        level: GetBreadcrumbLevel(logEvent)
                                    )
                                );
                            }
                        }
                        catch (Exception)
                        {
                            span.Status = SpanStatus.InternalError;
                            Console.WriteLine("Failed to capture event");
                            throw;
                        }
                        finally
                        {
                            scope.AddBreadcrumb(
                                new Breadcrumb(
                                    message: span.Description ?? string.Empty,
                                    type: "span",
                                    data: SpanTagsToStringDictionary(span.Tags),
                                    category: span.Operation,
                                    level: BreadcrumbLevel.Info
                                )
                            );
                            span.Finish();
                        }
                    });
                }
            }
            finally
            {
                if (transaction != null && transaction.Status == null)
                {
                    transaction.Finish(SpanStatus.UnknownError);
                }
                else
                {
                    transaction?.Finish();
                }
            }
        }

        /// <summary>
        /// Captures the log event without starting a performance transaction or span (when tracing is disabled).
        /// </summary>
        private void EmitWithoutTracing(LogEvent logEvent, SentryLevel level)
        {
            using (SentrySdk.PushScope())
            {
                SentrySdk.ConfigureScope(scope =>
                {
                    scope.Level = level;
                    scope.SetExtras(logEvent.Properties.Where(pair => _tags.All(t => t != pair.Key))
                        .ToDictionary(pair => pair.Key, pair => (object?)(Render(pair.Value, _formatProvider) ?? string.Empty)));
                    scope.SetTags(
                        RenderPropertyValuesToStringDictionary(
                            logEvent.Properties.Where(pair => _tags.Any(t => t == pair.Key)),
                            _formatProvider));

                    if (_tags.Length > 0)
                    {
                        scope.SetTags(_tags.Select(t => t.Split('='))
                            .Where(t => t.Length == 2)
                            .ToDictionary(t => t[0], t => t[1]));
                    }

                    scope.SetExtras((logEvent.Exception?.Data ?? new Dictionary<string, object>())
                        .Cast<KeyValuePair<string, object?>>());

                    scope.SetExtras(logEvent.MessageTemplate.Tokens
                        .OfType<PropertyToken>()
                        .Select(pt => new KeyValuePair<string, object?>(pt.PropertyName, logEvent.Properties[pt.PropertyName]))
                        .Where(pair => _tags.All(t => t != pair.Key))
                        .ToDictionary(pair => pair.Key, pair => pair.Value));

                    scope.AddBreadcrumb(
                        new Breadcrumb(
                            message: logEvent.MessageTemplate.Text,
                            type: "log",
                            data: RenderPropertyValuesToStringDictionary(logEvent.Properties, _formatProvider),
                            category: logEvent.Level.ToString(),
                            level: GetBreadcrumbLevel(logEvent)
                        ));

                    if (logEvent.Exception == null)
                    {
                        _sentrySdkWrapper.CaptureMessage(logEvent.MessageTemplate.Text, level);
                    }
                    else
                    {
                        _sentrySdkWrapper.CaptureException(logEvent.Exception);
                        scope.SetTag("exception_type", logEvent.Exception.GetType().Name);
                    }
                });
            }
        }

        /// <summary>
        /// Maps a Serilog event level to a Sentry level.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>The Sentry BreadcrumbLevel level.</returns>
        private BreadcrumbLevel GetBreadcrumbLevel(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                return BreadcrumbLevel.Error;
            }

            return logEvent.Level switch
            {
                LogEventLevel.Verbose => BreadcrumbLevel.Debug,
                LogEventLevel.Debug => BreadcrumbLevel.Debug,
                LogEventLevel.Information => BreadcrumbLevel.Info,
                LogEventLevel.Warning => BreadcrumbLevel.Warning,
                LogEventLevel.Error => BreadcrumbLevel.Error,
                LogEventLevel.Fatal => BreadcrumbLevel.Fatal,
                _ => BreadcrumbLevel.Error
            };
        }

        /// <summary>
        /// Maps a Serilog event level to a Sentry level.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>The Sentry level.</returns>
        static SentryLevel GetSentryLevel(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                return SentryLevel.Error;
            }

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

        static Dictionary<string, string> RenderPropertyValuesToStringDictionary(
            IEnumerable<KeyValuePair<string, LogEventPropertyValue>> pairs,
            IFormatProvider? formatProvider)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var pair in pairs)
            {
                dictionary[pair.Key] = Render(pair.Value, formatProvider) ?? string.Empty;
            }

            return dictionary;
        }

        static Dictionary<string, string> ExceptionDataToStringDictionary(IDictionary data)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (DictionaryEntry entry in data)
            {
                var key = entry.Key?.ToString();
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                dictionary[key] = entry.Value?.ToString() ?? string.Empty;
            }

            return dictionary;
        }

        static Dictionary<string, string> SpanTagsToStringDictionary(IReadOnlyDictionary<string, string>? tags)
        {
            var dictionary = new Dictionary<string, string>();
            if (tags == null)
            {
                return dictionary;
            }

            foreach (var pair in tags)
            {
                dictionary[pair.Key] = pair.Value ?? string.Empty;
            }

            return dictionary;
        }

        /// <summary>
        /// Renders the log event property value as a string.
        /// </summary>
        /// <param name="logEventPropertyValue">The log event property value.</param>
        /// <param name="formatProvider">The format provider, or null.</param>
        /// <returns>A string representation of the log event property value.</returns>
        static string? Render(LogEventPropertyValue logEventPropertyValue, IFormatProvider? formatProvider)
        {
            if (logEventPropertyValue is ScalarValue scalarValue && scalarValue.Value is string stringValue)
            {
                // Remove quotes from the value
                return stringValue;
            }

            return logEventPropertyValue?.ToString(null, formatProvider);
        }

        /// <summary>
        /// Disposes the sink.
        /// </summary>
        public void Dispose()
        {
            _sentry?.Dispose();
        }

    }
}
