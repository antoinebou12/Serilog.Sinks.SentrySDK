using Serilog;
using Serilog.Sink.Sentry;

using Xunit;

namespace Serilog.Sink.Sentry.Tests
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
