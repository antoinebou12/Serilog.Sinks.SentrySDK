using System.Security.Claims;

using Moq;

using Sentry;

using Serilog;
using Serilog.Sink.Sentry;

using Xunit;

namespace Serilog.Sink.Sentry.Tests
{
    public class SentryUserFactoryTests
    {
        private readonly Mock<ISentryHttpContext> _httpContextMock;
        private readonly Mock<ClaimsIdentity> _claimsIdentityMock;
        private readonly SentryUserFactory _factory;

        public SentryUserFactoryTests()
        {
            _httpContextMock = new Mock<ISentryHttpContext>();
            _claimsIdentityMock = new Mock<ClaimsIdentity>();
            _factory = new SentryUserFactory(_httpContextMock.Object);
        }

        [Fact]
        public void TestCreateUser()
        {
            // Arrange
            _httpContextMock.Setup(hc => hc.RemoteIpAddress).Returns("127.0.0.1");
            _claimsIdentityMock.Setup(ci => ci.FindFirst(ClaimTypes.Name)).Returns(new Claim(ClaimTypes.Name, "testuser"));
            _claimsIdentityMock.Setup(ci => ci.FindFirst(ClaimTypes.Email)).Returns(new Claim(ClaimTypes.Email, "testuser@example.com"));
            _httpContextMock.Setup(hc => hc.User.Identity).Returns(_claimsIdentityMock.Object);

            // Act
            User user = _factory.Create();

            // Assert
            Assert.Equal("127.0.0.1", user.IpAddress);
            Assert.Equal("testuser", user.Username);
            Assert.Equal("testuser@example.com", user.Email);
        }
    }
}
