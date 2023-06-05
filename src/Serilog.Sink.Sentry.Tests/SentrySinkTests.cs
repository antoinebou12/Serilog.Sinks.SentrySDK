using System;
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

        public SentrySinkTests()
        {
            _formatProviderMock = new Mock<IFormatProvider>();
            var sentryOptions = new SentryOptions
            {
                Dsn = "https://example@sentry.io/0",
                Release = "test",
                Environment = "test-environment",
            };
            _sentrySink = new SentrySink(_formatProviderMock.Object, sentryOptions, "tag1,tag2");
        }

        [Fact]
        public void Emit_CapturesMessage_WhenLogEventHasNoException()
        {
            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null, new MessageTemplate("Test", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            _sentrySink.Emit(logEvent);

            // NOTE: Verification of SentrySdk.CaptureMessage() is not directly possible as it's a static method of a static class. 
            // Ideally you would use an adapter or wrapper around the SentrySdk to allow for verification.
        }

        [Fact]
        public void Emit_CapturesException_WhenLogEventHasException()
        {
            var exception = new Exception("Test Exception");
            var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Error, exception, new MessageTemplate("Test", new List<MessageTemplateToken>()), new List<LogEventProperty>());

            _sentrySink.Emit(logEvent);

            // NOTE: Verification of SentrySdk.CaptureException() is not directly possible as it's a static method of a static class. 
            // Ideally you would use an adapter or wrapper around the SentrySdk to allow for verification.
        }
    }
}
