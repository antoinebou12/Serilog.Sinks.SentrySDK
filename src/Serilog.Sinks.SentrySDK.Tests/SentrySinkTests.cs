using System;

using System.Collections.Generic;
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
        private readonly Mock<ITransaction> _transactionMock;
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<ISpan> _spanMock;

        public SentrySinkTests()
        {
            _formatProviderMock = new Mock<IFormatProvider>();
            _transactionServiceMock = new Mock<ITransactionService>();
            _transactionMock = new Mock<ITransaction>();
            _spanMock = new Mock<ISpan>();
            _transactionServiceMock.Setup(s => s.StartChild(It.IsAny<ITransaction>(), It.IsAny<string>(), It.IsAny<string>())).Returns(_spanMock.Object);
            _sentrySdkWrapperMock = new Mock<ISentrySdkWrapper>();
            _sentrySdkWrapperMock
                        .Setup(sdk => sdk.StartTransaction(It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(_transactionMock.Object);


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
                EnableTracing = true,
                TracesSampleRate = 1.0,
                StackTraceMode = StackTraceMode.Enhanced
            };

            _sentrySink = new SentrySink(
                formatProvider: _formatProviderMock.Object,
                sentrySdkWrapper: _sentrySdkWrapperMock.Object,
                dsn: sentryOptions.Dsn,
                tags: "tag1,tag2",
                attachStacktrace: sentryOptions.AttachStacktrace,
                includeActivityData: true,
                sendDefaultPii: sentryOptions.SendDefaultPii,
                maxBreadcrumbs: sentryOptions.MaxBreadcrumbs,
                maxQueueItems: sentryOptions.MaxQueueItems,
                debug: sentryOptions.Debug,
                diagnosticLevel: sentryOptions.DiagnosticLevel.ToString(),
                environment: sentryOptions.Environment,
                serverName: sentryOptions.ServerName,
                release: sentryOptions.Release,
                restrictedToMinimumLevel: LogEventLevel.Error,
                transactionName: null,
                operationName: null,
                sampleRate: (float) sentryOptions.SampleRate,
                autoSessionTracking: true,
                enableTracing: true,
                transactionService: _transactionServiceMock.Object,
                tracesSampleRate: (float) sentryOptions.TracesSampleRate,
                stackTraceMode: "Enhanced"
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
            // Arrange
            LogEvent nullLogEvent = null;

            // Act
            var ex = Record.Exception(() => _sentrySink.Emit(nullLogEvent));

            // Assert
            Assert.Null(ex);
        }
    }
}
