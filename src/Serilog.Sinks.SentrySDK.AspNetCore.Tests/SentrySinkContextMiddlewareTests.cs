using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Moq;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SentrySDK.AspNetCore;

using Xunit;

namespace Serilog.Sinks.SentrySDK.AspNetCore.Tests
{
    public class SentrySinkContextMiddlewareTests
    {
        public class DelegatingSink : ILogEventSink
        {
            private readonly Action<LogEvent> _write;

            public DelegatingSink(Action<LogEvent> write)
            {
                _write = write;
            }

            public void Emit(LogEvent logEvent)
            {
                _write(logEvent);
            }
        }

        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<HttpContext> _contextMock;

        public SentrySinkContextMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _contextMock = new Mock<HttpContext>();
        }

        [Fact]
        public async Task Invoke_CallsNextDelegate()
        {
            var middleware = new SentrySinkContextMiddleware(_nextMock.Object);

            await middleware.Invoke(_contextMock.Object);

            _nextMock.Verify(next => next(_contextMock.Object), Times.Once);
        }

        [Fact]
        public async Task Invoke_LogsException_WhenNextDelegateThrows()
        {
            var middleware = new SentrySinkContextMiddleware(_nextMock.Object);

            _nextMock.Setup(next => next(_contextMock.Object)).ThrowsAsync(new Exception("Test exception"));

            // Arrange
            var logEvents = new List<LogEvent>();
            var logger = new LoggerConfiguration()
                .WriteTo.Sink(new DelegatingSink(le => logEvents.Add(le)))
                .CreateLogger();
            Log.Logger = logger;

            // Act
            try
            {
                await middleware.Invoke(_contextMock.Object);
            }
            catch
            {
                // ignored
            }

            // Assert
            Assert.Single(logEvents);
            Assert.Contains(logEvents, le => le.Exception.Message == "Test exception");
        }
    }
}
