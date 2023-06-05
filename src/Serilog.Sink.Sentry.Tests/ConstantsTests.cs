using Serilog;
using Serilog.Sinks.SentrySDK;

using Xunit;

namespace Serilog.Sinks.SentrySDK.Tests
{
    public class SentrySinkConstantsTests
    {
        [Fact]
        public void TestHttpContextKey()
        {
            Assert.Equal("SinkHttpContext", SentrySinkConstants.HttpContextKey);
        }
    }
}
