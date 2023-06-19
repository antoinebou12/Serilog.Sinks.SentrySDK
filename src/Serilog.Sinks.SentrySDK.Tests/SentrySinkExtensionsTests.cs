using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Moq;

using Ion.Framework.Serilog.Sinks.Sentry;

using Xunit;

namespace Ion.Framework.Serilog.Sinks.Sentry
{
    public class SentrySinkContextMiddlewareExtensionsTests
    {
        private readonly Mock<IApplicationBuilder> _appMock;

        public SentrySinkContextMiddlewareExtensionsTests()
        {
            _appMock = new Mock<IApplicationBuilder>();
        }

        [Fact]
        public void AddSentryContext_ShouldUseMiddleware()
        {
            _appMock.Setup(app => app.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns(_appMock.Object);

            var app = _appMock.Object.AddSentryContext();

            _appMock.Verify(app => app.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Exactly(2));

            Assert.Equal(_appMock.Object, app);
        }
    }
}
