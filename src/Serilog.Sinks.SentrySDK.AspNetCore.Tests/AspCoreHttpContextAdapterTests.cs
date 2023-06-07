using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Moq;

using Serilog.Sinks.SentrySDK;
using Serilog.Sinks.SentrySDK.AspNetCore;

using Xunit;

namespace Serilog.Sinks.SentrySDK.AspNetCore.Tests
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
            var expected = "127.0.0.1";

            _httpContext.Connection.RemoteIpAddress = IPAddress.Parse(expected);
            Assert.Equal(expected, _aspCoreHttpContextAdapter.RemoteIpAddress);
        }

        [Fact]
        public void RequestCookies_ReturnsCorrectCookies()
        {
            var expectedCookies = new Dictionary<string, string> { { "cookie1", "value1" }, { "cookie2", "value2" } };

            _httpContext.Request.Headers["Cookie"] = new StringValues(expectedCookies.Select(c => $"{c.Key}={c.Value}").ToArray());

            var resultCookies = _aspCoreHttpContextAdapter.RequestCookies;

            Assert.Equal(expectedCookies.Count, resultCookies.Count);
            Assert.True(expectedCookies.All(c => resultCookies.ContainsKey(c.Key) && resultCookies[c.Key] == c.Value));
        }

        [Fact]
        public void RequestHeaders_ReturnsCorrectHeaders()
        {
            _httpContext.Request.Headers.Add("header1", new StringValues("value1"));
            _httpContext.Request.Headers.Add("header2", new StringValues("value2 value3"));

            var resultHeaders = _aspCoreHttpContextAdapter.RequestHeaders;

            Assert.Equal(_httpContext.Request.Headers.Count, resultHeaders.Count);
            Assert.True(_httpContext.Request.Headers.All(h => resultHeaders.ContainsKey(h.Key) && resultHeaders[h.Key] == h.Value));
        }

        [Fact]
        public void RequestMethod_ReturnsCorrectMethod()
        {
            var expected = "GET";
            _httpContext.Request.Method = expected;

            Assert.Equal(expected, _aspCoreHttpContextAdapter.RequestMethod);
        }

        [Fact]
        public void RequestPath_ReturnsCorrectPath()
        {
            var expected = "/path1";
            _httpContext.Request.Path = expected;

            Assert.Equal(expected, _aspCoreHttpContextAdapter.RequestPath);
        }

        [Fact]
        public void RequestQueryString_ReturnsCorrectQueryString()
        {
            var expected = "?param1=value1&param2=value2";
            _httpContext.Request.QueryString = new QueryString(expected);

            Assert.Equal(expected, _aspCoreHttpContextAdapter.RequestQueryString);
        }

        [Fact]
        public void User_ReturnsCorrectUser()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "Test User") };
            var identity = new ClaimsIdentity(claims, "Test Auth Type");
            var expectedUser = new ClaimsPrincipal(identity);
            _httpContext.User = expectedUser;

            Assert.Equal(expectedUser as IPrincipal, _aspCoreHttpContextAdapter.User);
        }

        [Fact]
        public void GetRequestBody_ReturnsCorrectRequestBody_WhenBodyCanSeek()
        {
            var expectedBody = "This is a request body.";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(expectedBody);
            writer.Flush();
            memoryStream.Position = 0;
            _httpContext.Request.Body = memoryStream;

            var resultBody = _aspCoreHttpContextAdapter.GetRequestBody();

            Assert.Equal(expectedBody, resultBody);
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

        [Fact]
        public void RequestProtocol_ReturnsCorrectProtocol()
        {
            var expected = "HTTP/1.1";
            _httpContext.Request.Protocol = expected;

            Assert.Equal(expected, _aspCoreHttpContextAdapter.RequestProtocol);
        }

        [Fact]
        public void RequestScheme_ReturnsCorrectScheme()
        {
            var expected = "https";
            _httpContext.Request.Scheme = expected;

            Assert.Equal(expected, _aspCoreHttpContextAdapter.RequestScheme);
        }

        [Fact]
        public void RequestUserAgent_ReturnsCorrectUserAgent()
        {
            var expected = "TestAgent";
            _httpContext.Request.Headers.Add("User-Agent", new StringValues(expected));

            Assert.Equal(expected, _aspCoreHttpContextAdapter.RequestUserAgent);
        }

        [Fact]
        public void ResponseStatusCode_ReturnsCorrectStatusCode()
        {
            var expected = 200;
            _httpContext.Response.StatusCode = expected;

            Assert.Equal(expected, _aspCoreHttpContextAdapter.ResponseStatusCode);
        }

        [Fact]
        public async Task GetRequestBodyAsync_ReturnsCorrectRequestBody_WhenBodyCanSeek()
        {
            var expectedBody = "This is a request body.";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            await writer.WriteAsync(expectedBody);
            await writer.FlushAsync();
            memoryStream.Position = 0;
            _httpContext.Request.Body = memoryStream;

            var resultBody = await _aspCoreHttpContextAdapter.GetRequestBodyAsync();

            Assert.Equal(expectedBody, resultBody);
        }

        [Fact]
        public async Task GetRequestBodyAsync_ReturnsNull_WhenBodyCannotSeek()
        {
            var mockStream = new Mock<Stream>();
            mockStream.Setup(m => m.CanSeek).Returns(false);
            _httpContext.Request.Body = mockStream.Object;

            var resultBody = await _aspCoreHttpContextAdapter.GetRequestBodyAsync();

            Assert.Null(resultBody);
        }

    }
}