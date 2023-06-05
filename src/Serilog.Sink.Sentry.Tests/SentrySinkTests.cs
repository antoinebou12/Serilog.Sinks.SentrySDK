using Moq;

using Sentry;

using Serilog.Events;
using Serilog.Sink.Sentry;

using Xunit;

namespace Serilog.Sink.Sentry.Tests
{
    public class SentrySinkTests
    {
        private readonly SentrySink _sentrySink;
        private readonly Mock<IFormatProvider> _formatProviderMock;
        private readonly Mock<ISentryClient> _sentryClientMock;

        public SentrySinkTests()
        {
            _formatProviderMock = new Mock<IFormatProvider>();
            _sentryClientMock = new Mock<ISentryClient>();
            // Ideally, you should mock SentryOptions and make sure it returns _sentryClientMock when .SentryClient is called

            // Assuming that your SentrySink takes SentryOptions as argument
            _sentrySink = new SentrySink(_formatProviderMock.Object, new SentryOptions());
        }

        [Fact]
        public void TestEmit_CapturesException_WhenLogEventHasException()
        {
            var exception = new Exception("Test Exception");
            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Error, exception, new MessageTemplate("Test", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            _sentrySink.Emit(logEvent);

            _sentryClientMock.Verify(c => c.CaptureException(It.Is<SentryEvent>(e => e.Exception == exception)), Times.Once);
        }

        [Fact]
        public void TestEmit_CapturesMessage_WhenLogEventHasNoException()
        {
            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null, new MessageTemplate("Test", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            _sentrySink.Emit(logEvent);

            _sentryClientMock.Verify(c => c.CaptureMessage(It.Is<SentryEvent>(e => e.Message == "Test")), Times.Once);
        }
    }
}
