#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using Moq;
using Sentry;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.SentrySDK;
using Xunit;

namespace Serilog.Sinks.SentrySDK.Tests
{
    public class SentrySinkTests
    {
        private readonly SentrySink _sentrySink;
        private readonly Mock<ISentrySdkWrapper> _sentrySdkWrapperMock;
        private readonly Mock<IFormatProvider> _formatProviderMock;
        private readonly Mock<ITransactionTracer> _transactionMock;
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<ISpan> _spanMock;

        public SentrySinkTests()
        {
            _formatProviderMock = new Mock<IFormatProvider>();
            _transactionServiceMock = new Mock<ITransactionService>();
            _transactionMock = new Mock<ITransactionTracer>();
            _spanMock = new Mock<ISpan>();
            _transactionServiceMock.Setup(s => s.StartChild(It.IsAny<ITransactionTracer>(), It.IsAny<string>(), It.IsAny<string>())).Returns(_spanMock.Object);
            _sentrySdkWrapperMock = new Mock<ISentrySdkWrapper>();
            _sentrySdkWrapperMock
                        .Setup(sdk => sdk.StartTransaction(It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(_transactionMock.Object);

            _transactionMock.Setup(t => t.Status).Returns((SpanStatus?)null);
            _spanMock.SetupProperty(s => s.Status);
            _spanMock.Setup(s => s.SpanId).Returns(default(SpanId));
            _spanMock.Setup(s => s.Description).Returns(string.Empty);
            _spanMock.Setup(s => s.Tags).Returns(new Dictionary<string, string>());
            _spanMock.Setup(s => s.Operation).Returns("test-op");

            var sentryOptions = new SentryOptions
            {
                Dsn = "https://example@sentry.io/0",
                Release = "test",
                Environment = "test-environment",
                Debug = true,
                AttachStacktrace = true,
                SendDefaultPii = true,
                MaxBreadcrumbs = 50,
                MaxQueueItems = 100,
                DiagnosticLevel = SentryLevel.Warning,
                SampleRate = 1.0f,
                AutoSessionTracking = true,
                TracesSampleRate = 1.0,
                StackTraceMode = StackTraceMode.Enhanced,
                ServerName = "test-server",
                ShutdownTimeout = TimeSpan.FromSeconds(2.0),
                MaxCacheItems = 30,
                Distribution = "unspecified",
                IsEnvironmentUser = true
            };

            _sentrySink = new SentrySink(
                formatProvider: _formatProviderMock.Object,
                sentrySdkWrapper: _sentrySdkWrapperMock.Object,
                dsn: sentryOptions.Dsn,
                tags: "tag1=tag1,tag2=tag2",
                attachStacktrace: sentryOptions.AttachStacktrace,
                sendDefaultPii: sentryOptions.SendDefaultPii,
                maxBreadcrumbs: sentryOptions.MaxBreadcrumbs,
                maxQueueItems: sentryOptions.MaxQueueItems,
                debug: sentryOptions.Debug,
                diagnosticLevel: sentryOptions.DiagnosticLevel.ToString(),
                environment: sentryOptions.Environment,
                serverName: sentryOptions.ServerName,
                release: sentryOptions.Release,
                restrictedToMinimumLevel: LogEventLevel.Verbose.ToString(),
                transactionName: null,
                operationName: null,
                sampleRate: (float)sentryOptions.SampleRate,
                autoSessionTracking: true,
                enableTracing: true,
                transactionService: _transactionServiceMock.Object,
                tracesSampleRate: (float)sentryOptions.TracesSampleRate,
                stackTraceMode: "Enhanced",
                isEnvironmentUser: true,
                shutdownTimeout: 2.0,
                maxCacheItems: 30,
                distribution: "unspecified",
                configureSentryOptions: null,
                beforeSend: null
            );

        }

        [Fact]
        public void Emit_CapturesMessage_WhenLogEventHasNoException()
        {
            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null, new MessageTemplate("Test", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            _sentrySink.Emit(logEvent);

            _sentrySdkWrapperMock.Verify(sdk => sdk.CaptureMessage(It.IsAny<string>(), It.IsAny<SentryLevel>()), Times.Once);
        }

        [Fact]
        public void Emit_CapturesException_WhenLogEventHasException()
        {
            var exception = new Exception("Test Exception");
            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Error, exception, new MessageTemplate("Test", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            _sentrySink.Emit(logEvent);

            _sentrySdkWrapperMock.Verify(sdk => sdk.CaptureException(It.IsAny<Exception>()), Times.Once);
        }

        [Fact]
        public void Emit_WhenLogEventHasNoException_CallsCaptureMessageWithCorrectValues()
        {
            // Arrange
            var expectedMessage = "Test";
            var messageTemplate = new MessageTemplate(expectedMessage, new List<MessageTemplateToken>());
            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null, messageTemplate, new List<LogEventProperty>());

            // Act
            _sentrySink.Emit(logEvent);

            // Assert
            _sentrySdkWrapperMock.Verify(sdk => sdk.CaptureMessage(expectedMessage, It.IsAny<SentryLevel>()), Times.Once);
        }

        [Fact]
        public void Emit_WhenLogEventHasException_CallsCaptureExceptionWithCorrectValues()
        {
            // Arrange
            var expectedException = new Exception("Test Exception");
            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Error, expectedException, new MessageTemplate("Test", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            // Act
            _sentrySink.Emit(logEvent);

            // Assert
            _sentrySdkWrapperMock.Verify(sdk => sdk.CaptureException(expectedException), Times.Once);
        }

        [Fact]
        public void Emit_ShouldNotThrowException_WhenLogEventIsNull()
        {
            var ex = Record.Exception(() => _sentrySink.Emit(null!));

            Assert.Null(ex);
        }

        [Fact]
        public void Emit_WhenTracingDisabled_DoesNotStartTransaction_CapturesMessage()
        {
            var wrapper = new Mock<ISentrySdkWrapper>();
            wrapper.Setup(s => s.StartTransaction(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new InvalidOperationException("StartTransaction should not be used when tracing is off"));

            using var sink = CreateTestSentrySink(wrapper.Object, enableTracing: false);

            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null,
                new MessageTemplate("NoTrace", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            sink.Emit(logEvent);

            wrapper.Verify(s => s.StartTransaction(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            wrapper.Verify(s => s.CaptureMessage("NoTrace", It.IsAny<SentryLevel>()), Times.Once);
        }

        [Fact]
        public void Emit_WhenTracingDisabled_CapturesException()
        {
            var wrapper = new Mock<ISentrySdkWrapper>();
            using var sink = CreateTestSentrySink(wrapper.Object, enableTracing: false);

            var ex = new Exception("offline");
            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Error, ex,
                new MessageTemplate("Err", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            sink.Emit(logEvent);

            wrapper.Verify(s => s.StartTransaction(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            wrapper.Verify(s => s.CaptureException(ex), Times.Once);
        }

        [Fact]
        public void Constructor_InvokesConfigureSentryOptions()
        {
            var configured = false;
            using var sink = CreateTestSentrySink(configureSentryOptions: _ => configured = true);
            Assert.True(configured);
        }

        [Fact]
        public void BeforeSendPipeline_WithEventIdInExceptionData_RemapsExtras()
        {
            using var sink = CreateTestSentrySink(beforeSend: null);

            var ex = new Exception("with id");
            ex.Data["EventId"] = "evt-99";
            ex.Data["Other"] = "keep";

            var incoming = new SentryEvent(ex);
            var result = InvokeBeforeSendInternal(sink, incoming, new SentryHint());

            Assert.NotNull(result);
            Assert.NotNull(result!.Exception);
            Assert.Equal("evt-99", result.Extra["EventId"]?.ToString());
            Assert.Equal("keep", result.Extra["Other"]?.ToString());
        }

        [Fact]
        public void BeforeSendPipeline_UserCallbackReceivesProcessedEvent()
        {
            SentryEvent? seen = null;
            using var sink = CreateTestSentrySink(beforeSend: (e, _) =>
            {
                seen = e;
                return e;
            });

            var ex = new Exception("x");
            ex.Data["EventId"] = "1";
            var incoming = new SentryEvent(ex);

            InvokeBeforeSendInternal(sink, incoming, new SentryHint());

            Assert.NotNull(seen);
            Assert.Equal("1", seen!.Extra["EventId"]?.ToString());
        }

        [Fact]
        public void BeforeSendPipeline_UserCallbackCanDropEvent()
        {
            using var sink = CreateTestSentrySink(beforeSend: (_, _) => null);

            var incoming = new SentryEvent(new Exception("drop me"));
            var result = InvokeBeforeSendInternal(sink, incoming, new SentryHint());

            Assert.Null(result);
        }

        [Fact]
        public void Constructor_UsesBuiltInTransactionService_WhenTransactionServiceIsNull()
        {
            var wrapper = new Mock<ISentrySdkWrapper>();
            using var sink = CreateTestSentrySink(
                wrapper.Object,
                enableTracing: false,
                useBuiltInTransactionService: true);

            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null,
                new MessageTemplate("BuiltInTx", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            sink.Emit(logEvent);

            wrapper.Verify(s => s.StartTransaction(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            wrapper.Verify(s => s.CaptureMessage("BuiltInTx", It.IsAny<SentryLevel>()), Times.Once);
        }

        [Fact]
        public void Constructor_DoesNotSetMaxCacheItems_WhenMaxCacheItemsIsZero()
        {
            using var sink = CreateTestSentrySink(enableTracing: false, maxCacheItems: 0);
            var options = GetSentryOptionsFromSink(sink);

            Assert.Equal(new SentryOptions().MaxCacheItems, options.MaxCacheItems);
        }

        [Fact]
        public void BeforeSendPipeline_NullIncomingEvent_ShortCircuitsBeforeUserCallback()
        {
            var userBeforeSendInvoked = false;
            using var sink = CreateTestSentrySink(beforeSend: (e, _) =>
            {
                userBeforeSendInvoked = true;
                return e;
            });

            var result = InvokeBeforeSendInternal(sink, null!, new SentryHint());

            Assert.Null(result);
            Assert.False(userBeforeSendInvoked);
        }

        [Fact]
        public void BeforeSendPipeline_EventIdExtraIsEmptyString_WhenEventIdDataValueIsNull()
        {
            using var sink = CreateTestSentrySink(beforeSend: null);

            var ex = new Exception("evt");
            ex.Data["EventId"] = null;

            var incoming = new SentryEvent(ex);
            var result = InvokeBeforeSendInternal(sink, incoming, new SentryHint());

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result!.Extra["EventId"]?.ToString());
        }

        [Fact]
        public void BeforeSendPipeline_SkipsExceptionDataEntriesWithEmptyKeyStringAlongsideEventId()
        {
            using var sink = CreateTestSentrySink(beforeSend: null);

            var ex = new Exception("keys");
            ex.Data[new DictionaryKeyWithEmptyStringName()] = "ignored";
            ex.Data["EventId"] = "keep-id";

            var incoming = new SentryEvent(ex);
            var result = InvokeBeforeSendInternal(sink, incoming, new SentryHint());

            Assert.NotNull(result);
            Assert.False(result!.Extra.ContainsKey(string.Empty));
            Assert.Equal("keep-id", result.Extra["EventId"]?.ToString());
        }

        [Fact]
        public void BeforeSendPipeline_MapsNullNonEventIdDataToEmptyExtraString()
        {
            using var sink = CreateTestSentrySink(beforeSend: null);

            var ex = new Exception("null other");
            ex.Data["EventId"] = "1";
            ex.Data["Other"] = null;

            var incoming = new SentryEvent(ex);
            var result = InvokeBeforeSendInternal(sink, incoming, new SentryHint());

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result!.Extra["Other"]?.ToString());
        }

        [Fact]
        public void Emit_WithTracing_UsesEmptySpanDescriptionAndEmptyTags_WhenSpanHasNullDescriptionAndTags()
        {
            var parser = new MessageTemplateParser();
            var template = parser.Parse("Hello {Name}");
            var wrapper = new Mock<ISentrySdkWrapper>();
            var transaction = new Mock<ITransactionTracer>();
            wrapper.Setup(s => s.StartTransaction(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(transaction.Object);
            transaction.Setup(t => t.Status).Returns((SpanStatus?)null);

            using var sink = CreateTestSentrySink(
                wrapper.Object,
                enableTracing: true,
                configureSpan: m =>
                {
                    m.Setup(s => s.Tags).Returns(default(IReadOnlyDictionary<string, string>)!);
                    m.Setup(s => s.Description).Returns(default(string)!);
                });

            var logEvent = new LogEvent(
                DateTimeOffset.Now,
                LogEventLevel.Information,
                null,
                template,
                new[] { new LogEventProperty("Name", new ScalarValue("x")) });

            sink.Emit(logEvent);

            wrapper.Verify(s => s.CaptureMessage(It.IsAny<string>(), It.IsAny<SentryLevel>()), Times.Once);
        }

        [Fact]
        public void Emit_WithTracing_Exception_WhenExceptionDataHasEmptyKeyAndNullValue()
        {
            var parser = new MessageTemplateParser();
            var template = parser.Parse("Err {Code}");
            var ex = new Exception("boom");
            ex.Data[new DictionaryKeyWithEmptyStringName()] = "skip";
            ex.Data["Norm"] = null;

            var wrapper = new Mock<ISentrySdkWrapper>();
            var transaction = new Mock<ITransactionTracer>();
            wrapper.Setup(s => s.StartTransaction(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(transaction.Object);
            transaction.Setup(t => t.Status).Returns((SpanStatus?)null);

            using var sink = CreateTestSentrySink(wrapper.Object, enableTracing: true);

            var logEvent = new LogEvent(
                DateTimeOffset.Now,
                LogEventLevel.Error,
                ex,
                template,
                new[] { new LogEventProperty("Code", new ScalarValue(1)) });

            sink.Emit(logEvent);

            wrapper.Verify(s => s.CaptureException(ex), Times.Once);
        }

        [Fact]
        public void Emit_WithNullFormatProvider_RendersNonStringScalarWithoutThrowing()
        {
            var parser = new MessageTemplateParser();
            var template = parser.Parse("N={N}");
            var wrapper = new Mock<ISentrySdkWrapper>();

            using var sink = CreateTestSentrySink(
                wrapper.Object,
                enableTracing: false,
                useNullFormatProvider: true);

            var logEvent = new LogEvent(
                DateTimeOffset.Now,
                LogEventLevel.Information,
                null,
                template,
                new[] { new LogEventProperty("N", new ScalarValue(42)) });

            sink.Emit(logEvent);

            wrapper.Verify(s => s.CaptureMessage(It.IsAny<string>(), It.IsAny<SentryLevel>()), Times.Once);
        }

        private sealed class DictionaryKeyWithEmptyStringName
        {
            public override string ToString() => string.Empty;
        }

        private static SentrySink CreateTestSentrySink(
            ISentrySdkWrapper? wrapper = null,
            bool enableTracing = true,
            Action<SentryOptions>? configureSentryOptions = null,
            Func<SentryEvent, SentryHint, SentryEvent?>? beforeSend = null,
            bool useNullFormatProvider = false,
            bool useBuiltInTransactionService = false,
            int maxCacheItems = 30,
            Action<Mock<ISpan>>? configureSpan = null)
        {
            IFormatProvider? formatProvider = useNullFormatProvider ? null : new Mock<IFormatProvider>().Object;

            var spanMock = new Mock<ISpan>();
            spanMock.SetupProperty(s => s.Status);
            spanMock.Setup(s => s.SpanId).Returns(default(SpanId));
            spanMock.Setup(s => s.Description).Returns(string.Empty);
            spanMock.Setup(s => s.Tags).Returns(new Dictionary<string, string>());
            spanMock.Setup(s => s.Operation).Returns("test-op");
            configureSpan?.Invoke(spanMock);

            ITransactionService? transactionServiceForCtor;
            if (useBuiltInTransactionService)
            {
                transactionServiceForCtor = null;
            }
            else
            {
                var transactionService = new Mock<ITransactionService>();
                transactionService.Setup(s => s.StartChild(It.IsAny<ITransactionTracer>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(spanMock.Object);
                transactionServiceForCtor = transactionService.Object;
            }

            wrapper ??= new Mock<ISentrySdkWrapper>().Object;

            return new SentrySink(
                formatProvider: formatProvider,
                sentrySdkWrapper: wrapper,
                dsn: "https://example@sentry.io/0",
                tags: "",
                sendDefaultPii: true,
                maxBreadcrumbs: 50,
                maxQueueItems: 100,
                debug: true,
                diagnosticLevel: SentryLevel.Warning.ToString(),
                environment: "test",
                serverName: "srv",
                release: "1.0",
                restrictedToMinimumLevel: LogEventLevel.Verbose.ToString(),
                transactionName: null,
                operationName: null,
                sampleRate: 1f,
                attachStacktrace: true,
                autoSessionTracking: true,
                enableTracing: enableTracing,
                transactionService: transactionServiceForCtor,
                tracesSampleRate: 1.0,
                stackTraceMode: "Enhanced",
                isEnvironmentUser: true,
                shutdownTimeout: 2.0,
                maxCacheItems: maxCacheItems,
                distribution: "unspecified",
                configureSentryOptions: configureSentryOptions,
                beforeSend: beforeSend);
        }

        private static SentryOptions GetSentryOptionsFromSink(SentrySink sink)
        {
            var optionsField = typeof(SentrySink).GetField("_options", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(optionsField);
            return (SentryOptions)optionsField!.GetValue(sink)!;
        }

        private static SentryEvent? InvokeBeforeSendInternal(SentrySink sink, SentryEvent? @event, SentryHint hint)
        {
            var options = GetSentryOptionsFromSink(sink);

            var prop = typeof(SentryOptions).GetProperty("BeforeSendInternal", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(prop);
            var del = (Func<SentryEvent, SentryHint, SentryEvent?>?)prop!.GetValue(options);
            Assert.NotNull(del);
            return del!(@event!, hint);
        }
    }
}
