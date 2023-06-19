using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Sentry;

using Serilog.Core;
using Serilog.Events;

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
        ITransaction StartTransaction(string name, string operation);
    }

    /// <summary>
    /// SentrySdkWrapper
    /// </summary>
    /// <seealso cref="Serilog.Sinks.SentrySDK.ISentrySdkWrapper" />
    public class SentrySdkWrapper : ISentrySdkWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SentrySdkWrapper"/> class.
        /// </summary>
        /// <param name="options"></param>
        public SentrySdkWrapper(SentryOptions options)
        {
            SentrySdk.Init(options);
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
        public ITransaction StartTransaction(string name, string operation)
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
        /// Starts the child.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="description"></param>
        /// <returns></returns>
        ISpan StartChild(ITransaction transaction, string operation, string description);
    }

    /// <summary>
    /// TransactionService
    /// </summary>
    /// <seealso cref="Serilog.Sinks.SentrySDK.ITransactionService" />
    public class TransactionService : ITransactionService
    {
        /// <inheritdoc/>
        public ISpan StartChild(ITransaction transaction, string operation, string description)
        {
            return transaction.StartChild(operation, description);
        }
    }


    /// <summary>
    /// Provides a sink that directs log events to the Sentry service.
    /// </summary>
    public class SentrySink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly string[] _tags;
        private readonly SentryOptions _options;
        private readonly IDisposable _sentry;
        private readonly string _transactionName;
        private readonly string _operationName;
        private readonly ISentrySdkWrapper _sentrySdkWrapper;
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentrySink"/> class.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="sentrySdkWrapper"></param>
        /// <param name="dsn"></param>
        /// <param name="tags">The tags.</param>
        /// <param name="includeActivityData"></param>
        /// <param name="sendDefaultPii"></param>
        /// <param name="maxBreadcrumbs"></param>
        /// <param name="maxQueueItems"></param>
        /// <param name="debug"></param>
        /// <param name="diagnosticLevel"></param>
        /// <param name="environment"></param>
        /// <param name="serverName"></param>
        /// <param name="release"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="transactionName"></param>
        /// <param name="operationName"></param>
        /// <param name="sampleRate"></param>
        /// <param name="attachStacktrace"></param>
        /// <param name="autoSessionTracking"></param>
        /// <param name="enableTracing"></param>
        /// <param name="transactionService"></param>
        /// <param name="tracesSampleRate"></param>
        /// <param name="stackTraceMode"></param>
        /// <param name="sentryScopeStateProcessor"></param>
        /// <param name="isEnvironmentUser"></param>
        /// <param name="shutdownTimeout"></param>
        /// <param name="maxCacheItems"></param>
        /// <param name="distribution"></param>
        public SentrySink(IFormatProvider formatProvider, ISentrySdkWrapper sentrySdkWrapper, string dsn, string tags,
        bool includeActivityData, bool sendDefaultPii, int maxBreadcrumbs,
        int maxQueueItems, bool debug, string diagnosticLevel, string environment,
        string serverName, string release, LogEventLevel restrictedToMinimumLevel,
        string transactionName, string operationName, float sampleRate,
        bool attachStacktrace, bool autoSessionTracking, bool enableTracing,
        ITransactionService transactionService, double tracesSampleRate, string stackTraceMode,
        bool isEnvironmentUser,
        double shutdownTimeout,
        int maxCacheItems,
        string distribution,
        ISentryScopeStateProcessor sentryScopeStateProcessor = null)
        {
            _formatProvider = formatProvider;
            _sentrySdkWrapper = sentrySdkWrapper;
            _tags = string.IsNullOrWhiteSpace(tags) ? new string[0] : tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
            _transactionName = transactionName;
            _operationName = operationName;
            if (string.IsNullOrWhiteSpace(dsn))
            {
                Console.WriteLine(@"
                    // A DSN must be provided to the Sentry sink (Client Keys DSN). See https://docs.sentry.io/error-reporting/configuration/?platform=csharp for more information.
                    Add it in appsettings.json
                    ```
                    {
                        ""Serilog"": {
                            ""WriteTo"": [
                                {
                                    ""Name"": ""Sentry"",
                                    ""Args"": {
                                        ""dsn"": ""YourSentryDsn""
                                    }
                                }
                            ]
                        }
                    }
                    ```
                ");

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
                EnableTracing = enableTracing,
                ServerName = serverName ?? Environment.MachineName,
                TracesSampleRate = tracesSampleRate,
                StackTraceMode = Enum.Parse<StackTraceMode>(stackTraceMode, true),
                SentryScopeStateProcessor = sentryScopeStateProcessor,
                IsEnvironmentUser = isEnvironmentUser,
                ShutdownTimeout = TimeSpan.FromSeconds(shutdownTimeout),
                MaxCacheItems = maxCacheItems,
                Distribution = distribution,
            };
            _options.SetBeforeSend((sentryEvent, hint) =>
            {
                if (sentryEvent.Exception != null && sentryEvent.Exception.Data.Contains("EventId"))
                {
                    var newEvent = new SentryEvent(sentryEvent.Exception)
                    {
                        Level = sentryEvent.Level // Also copy the Level from the original event
                    };

                    foreach (System.Collections.DictionaryEntry entry in sentryEvent.Exception.Data)
                    {
                        var key = entry.Key.ToString();
                        if (key == "EventId")
                        {
                            newEvent.SetExtra(key, sentryEvent.Exception.Data["EventId"].ToString());
                        }
                        else
                        {
                            newEvent.SetExtra(key, entry.Value.ToString());
                        }
                    }
                    return newEvent;
                }
                return sentryEvent;
            });
            try
            {
                // Initialize the SDK
                _sentry = SentrySdk.Init(_options);

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
            var level = GetSentryLevel(logEvent);
            var transaction = _sentrySdkWrapper.StartTransaction(
                name: _transactionName,
                operation: _operationName
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

                    if (_tags.Length > 0)
                        {
                        scope.SetTags(_tags.Select(t => t.Split('='))
                            .Where(t => t.Length == 2)
                            .ToDictionary(t => t[0], t => t[1]));

                    }

                    var span = _transactionService.StartChild(
                        transaction: transaction,
                        operation: _operationName,
                        description: logEvent.RenderMessage(_formatProvider)
                    );
                    try
                    {
                        if (logEvent.Exception == null)
                        {
                            _sentrySdkWrapper.CaptureMessage(logEvent.MessageTemplate.Text, level);
                        }
                        else
                        {
                            _sentrySdkWrapper.CaptureException(logEvent.Exception);
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
                        span.Finish();
                    }
                });
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
        /// Maps a Serilog event level to a Sentry level.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>The Sentry level.</returns>
        private static SentryLevel GetSentryLevel(LogEvent logEvent)
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

        /// <summary>
        /// Disposes the sink.
        /// </summary>
        public void Dispose()
        {
            _sentry?.Dispose();
        }

    }
}
