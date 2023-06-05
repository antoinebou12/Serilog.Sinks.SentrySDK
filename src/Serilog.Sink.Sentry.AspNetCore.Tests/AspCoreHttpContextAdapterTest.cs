using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using Moq;

using Serilog.Sinks.SentrySDK.AspNetCore;

using Xunit;

namespace Serilog.Tests
{
    public class AspCoreHttpContextAdapterTests
    {
        private readonly DefaultHttpContext _httpContext;
        private readonly AspCoreHttpContextAdapter _aspCoreHttpContextAdapter;

        public AspCoreHttpContextAdapterTests()
        {
            _httpContext = new DefaultHttpContext();
            _aspCoreHttpContextAdapter = new AspCoreHttpContextAdapter(_httpContext);
        }

        [Fact]
        public void RemoteIpAddress_ReturnsCorrectIpAddress()
        {
            _httpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");

            Assert.Equal("127.0.0.1", _aspCoreHttpContextAdapter.RemoteIpAddress);
        }

        [Fact]
        public void RequestCookies_ReturnsCorrectCookies()
        {
            var cookies = new RequestCookieCollection(new Dictionary<string, string> { { "cookie1", "value1" }, { "cookie2", "value2" } });

            _httpContext.Request.Cookies = cookies;

            var resultCookies = _aspCoreHttpContextAdapter.RequestCookies;

            Assert.Equal(cookies.Count, resultCookies.Count);
            Assert.True(cookies.All(c => resultCookies.ContainsKey(c.Key) && resultCookies[c.Key] == c.Value));
        }

        [Fact]
        public void RequestHeaders_ReturnsCorrectHeaders()
        {
            _httpContext.Request.Headers.Add("header1", "value1");
            _httpContext.Request.Headers.Add("header2", "value2 value3");

            var resultHeaders = _aspCoreHttpContextAdapter.RequestHeaders;

            Assert.Equal(_httpContext.Request.Headers.Count, resultHeaders.Count);
            Assert.True(_httpContext.Request.Headers.All(h => resultHeaders.ContainsKey(h.Key) && resultHeaders[h.Key] == h.Value));
        }

        [Fact]
        public void RequestMethod_ReturnsCorrectMethod()
        {
            _httpContext.Request.Method = "GET";

            Assert.Equal("GET", _aspCoreHttpContextAdapter.RequestMethod);
        }

        [Fact]
        public void RequestPath_ReturnsCorrectPath()
        {
            _httpContext.Request.Path = "/path1";

            Assert.Equal("/path1", _aspCoreHttpContextAdapter.RequestPath);
        }

        [Fact]
        public void RequestQueryString_ReturnsCorrectQueryString()
        {
            _httpContext.Request.QueryString = new QueryString("?param1=value1&param2=value2");

            Assert.Equal("?param1=value1&param2=value2", _aspCoreHttpContextAdapter.RequestQueryString);
        }

        [Fact]
        public void User_ReturnsCorrectUser()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Test User") };
            var identity = new ClaimsIdentity(claims, "Test Auth Type");
            _httpContext.User = new ClaimsPrincipal(identity);

            Assert.Equal(_httpContext.User, _aspCoreHttpContextAdapter.User);
        }

        [Fact]
        public void GetRequestBody_ReturnsCorrectRequestBody_WhenBodyCanSeek()
        {
            var bodyString = "This is a request body.";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(bodyString);
            writer.Flush();
            memoryStream.Position = 0;
            _httpContext.Request.Body = memoryStream;

            var resultBody = _aspCoreHttpContextAdapter.GetRequestBody();

            Assert.Equal(bodyString, resultBody);
        }

        [Fact]
        public void GetRequestBody_ReturnsNull_WhenBodyCannotSeek()
        {
            var mockStream = new Mock<Stream>();
            mockStream.Setup(m => m.CanSeek).Returns(false);
            _httpContext.Request.Body = mockStream.Object;

            var resultBody = _aspCoreHttpContextAdapter.GetRequestBody();

            Assert.Null(resultBody);
        }
    }
}
