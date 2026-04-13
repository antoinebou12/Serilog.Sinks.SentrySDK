using Serilog;
using Serilog.Sinks.SentrySDK;
using Xunit;

namespace Serilog.Sinks.SentrySDK.Tests
{
    public class SentrySinkExtensionsTests
    {
        [Fact]
        public void Sentry_Extension_WiresSinkAndOptionalCallbacks()
        {
            var configureCalled = false;
            using var logger = new LoggerConfiguration()
                .WriteTo.Sentry(
                    "https://key@o.ingest.sentry.io/1",
                    configureSentryOptions: _ => configureCalled = true,
                    beforeSend: (e, _) => e,
                    enableTracing: false)
                .CreateLogger();

            Assert.True(configureCalled);
        }
    }
}
