extern alias SinkSentry;

using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using Moq;

using Serilog;

using Xunit;

namespace Serilog.Sink.Sentry.Tests
{
    public class SentryRequestFactoryTests
    {
        private readonly SinkSentry::Serilog.Sink.Sentry.ISentryRequestFactory _factory;
        private readonly Mock<SinkSentry::Serilog.Sink.Sentry.ISentryHttpContext> _httpContextMock;

        public SentryRequestFactoryTests()
        {
            _httpContextMock = new Mock<SinkSentry::Serilog.Sink.Sentry.ISentryHttpContext>();
            _factory = new SinkSentry::Serilog.Sink.Sentry.SentryRequestFactory(_httpContextMock.Object);
        }

        [Fact]
        public void Create_ShouldReturnCorrectSentryRequest()
        {
            // Arrange
            _httpContextMock.Setup(h => h.RequestPath).Returns("/test");
            _httpContextMock.Setup(h => h.RequestMethod).Returns("GET");
            _httpContextMock.Setup(h => h.RequestHeaders).Returns(new Dictionary<string, string> { {"TestHeader", "TestValue"} });
            _httpContextMock.Setup(h => h.RequestCookies).Returns(new Dictionary<string, string> { {"TestCookie", "TestValue"} });
            _httpContextMock.Setup(h => h.GetRequestBody()).Returns("RequestBody");
            _httpContextMock.Setup(h => h.RequestQueryString).Returns("?param=value");

            // Act
            var sentryRequest = _factory.Create();

            // Assert
            Assert.Equal("GET", sentryRequest.Method);
            Assert.Equal("/test", sentryRequest.Url);
            Assert.Contains(sentryRequest.Headers, h => h.Key == "TestHeader" && h.Value == "TestValue");
            Assert.Contains(sentryRequest.Cookies, h => h.Key == "TestCookie" && h.Value == "TestValue");
            Assert.Equal("RequestBody", sentryRequest.Data);
            Assert.Equal("?param=value", sentryRequest.QueryString);
        }
    }
}
