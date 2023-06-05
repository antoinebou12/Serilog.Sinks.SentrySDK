using System;

using Moq;

using Sentry;

using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.SentrySDK;

using Xunit;

namespace Serilog.Sinks.SentrySDK.Tests
{
    public class SentrySinkExtensionsTests
    {
        private readonly Mock<LoggerSinkConfiguration> _loggerConfigMock;
        private readonly string _dsn = "test_dsn";
        private readonly string _release = "test_release";
        private readonly string _environment = "test_environment";
        private readonly LogEventLevel _minimumLevel = LogEventLevel.Warning;

        public SentrySinkExtensionsTests()
        {
            _loggerConfigMock = new Mock<LoggerSinkConfiguration>();
        }

        [Fact]
        public void TestSentryConfiguration()
        {
            _loggerConfigMock.Object.Sentry(_dsn, _release, _environment, _minimumLevel);

            _loggerConfigMock.Verify(lc => lc.Sink(
                It.IsAny<SentrySink>(),
                _minimumLevel), Times.Once);
        }
    }
}
