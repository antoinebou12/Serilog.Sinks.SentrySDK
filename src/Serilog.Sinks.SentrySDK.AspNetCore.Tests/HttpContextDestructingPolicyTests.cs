using Moq;

using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SentrySDK;
using Serilog.Sinks.SentrySDK.AspNetCore;

using Xunit;

namespace Serilog.Sinks.SentrySDK.AspNetCore.Tests
{
    public class HttpContextDestructingPolicyTests
    {
        private readonly HttpContextDestructingPolicy _policy;
        private readonly Mock<ILogEventPropertyValueFactory> _valueFactoryMock;

        public HttpContextDestructingPolicyTests()
        {
            _policy = new HttpContextDestructingPolicy();
            _valueFactoryMock = new Mock<ILogEventPropertyValueFactory>();
        }

        [Fact]
        public void TryDestructure_ReturnsFalse_WhenValueIsNotISentryHttpContext()
        {
            object value = new object();

            bool result = _policy.TryDestructure(value, _valueFactoryMock.Object, out LogEventPropertyValue destructuredValue);

            Assert.False(result);
            Assert.Null(destructuredValue);
        }

        [Fact]
        public void TryDestructure_ReturnsTrue_WhenValueIsISentryHttpContext()
        {
            var sentryHttpContext = Mock.Of<ISentryHttpContext>();

            bool result = _policy.TryDestructure(sentryHttpContext, _valueFactoryMock.Object, out LogEventPropertyValue destructuredValue);

            Assert.True(result);
            Assert.IsType<ScalarValue>(destructuredValue);
            Assert.Equal(sentryHttpContext, ((ScalarValue)destructuredValue).Value);
        }
    }
}
