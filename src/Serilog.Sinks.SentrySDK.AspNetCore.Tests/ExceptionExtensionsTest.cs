using System;

using Serilog.Sinks.SentrySDK.AspNetCore;

using Xunit;

namespace Serilog.Tests
{
    public class ExceptionExtensionsTests
    {
        [Fact]
        public void CheckIfCaptured_ThrowsArgumentNullException_WhenExceptionIsNull()
        {
            Exception exception = null;

            Assert.Throws<ArgumentNullException>(() => exception.CheckIfCaptured());
        }

        [Fact]
        public void CheckIfCaptured_ReturnsFalse_WhenExceptionNotCaptured()
        {
            var exception = new Exception();

            Assert.False(exception.CheckIfCaptured());
        }

        [Fact]
        public void SetCaptured_DoesNotThrow_WhenExceptionIsNull()
        {
            Exception exception = null;

            var exceptionDuringSetCaptured = Record.Exception(() => exception.SetCaptured());
            Assert.Null(exceptionDuringSetCaptured);
        }

        [Fact]
        public void CheckIfCaptured_ReturnsTrue_WhenExceptionCaptured()
        {
            var exception = new Exception();
            exception.SetCaptured();

            Assert.True(exception.CheckIfCaptured());
        }
    }
}
