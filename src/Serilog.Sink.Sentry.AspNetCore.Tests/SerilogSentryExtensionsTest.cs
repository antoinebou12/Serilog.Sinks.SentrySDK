using Moq;
using Serilog.Configuration;
using Xunit;

namespace YourNamespace.Tests
{
    public class SerilogSentryExtensionsTests
    {
        private readonly Mock<LoggerSinkConfiguration> _loggerSinkConfigurationMock;
        private const string Dsn = "https://example@sentry.io/123";

        public SerilogSentryExtensionsTests()
        {
            _loggerSinkConfigurationMock = new Mock<LoggerSinkConfiguration>();
        }

        [Fact]
        public void Sentry_ShouldCreateSink_WhenDsnIsProvided()
        {
            _loggerSinkConfigurationMock.Setup(lsc => lsc.Sink(It.IsAny<SentrySink>(), It.IsAny<LogEventLevel>())).Returns(_loggerSinkConfigurationMock.Object);

            var result = _loggerSinkConfigurationMock.Object.Sentry(Dsn);

            _loggerSinkConfigurationMock.Verify(lsc => lsc.Sink(It.IsAny<SentrySink>(), It.IsAny<LogEventLevel>()), Times.Once);
            Assert.Equal(_loggerSinkConfigurationMock.Object, result);
        }

        [Fact]
        public void Sentry_ShouldThrowArgumentNullException_WhenLoggerConfigurationIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((LoggerSinkConfiguration)null).Sentry(Dsn));
        }

        [Fact]
        public void Sentry_ShouldThrowArgumentNullException_WhenDsnIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _loggerSinkConfigurationMock.Object.Sentry(null));
        }
    }
}
