using System;
using Xunit;
using Serilog.Events;
using Serilog;

namespace Serilog.Sinks.SentrySDK.Tests
{
    public class SentrySinkExtensionsTests
    {
        private readonly string _dsn = "your_sentry_dsn";
        private readonly string _release = "test_release";
        private readonly string _environment = "test_environment";
        private readonly LogEventLevel _minimumLevel = LogEventLevel.Warning;

        [Fact]
        public void TestSentryConfiguration()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Sentry(_dsn, _release, _environment, _minimumLevel)
                .CreateLogger();

            logger.Information("Test log");

            // This isn't a real assertion - replace it with a check that the log appeared in Sentry.
            // This will likely involve checking the Sentry API or your Sentry account.
            Assert.True(true);
        }
    }
}
