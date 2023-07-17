using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;

using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Serilog.Sinks.SentrySDK.AspNetCore
{
    /// <summary>
    /// Adapts an ASP.NET Core HttpContext to the Sentry SDK IHttpContext interface.
    /// </summary>
    public class AspCoreHttpContextAdapter : ISentryHttpContext
    {
        private readonly HttpContext _httpContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspCoreHttpContextAdapter"/> class.
        /// </summary>
        /// <param name="httpContext">The ASP.NET Core HttpContext to be adapted.</param>
        public AspCoreHttpContextAdapter(HttpContext httpContext)
        {
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        /// <summary>
        /// Gets the remote IP address.
        /// </summary>
        public string RemoteIpAddress => _httpContext.Connection.RemoteIpAddress.ToString();

        /// <summary>
        /// Gets the request cookies.
        /// </summary>
        public IDictionary<string, string> RequestCookies =>
            _httpContext.Request.Cookies.ToDictionary(x => x.Key, x => x.Value);

        /// <summary>
        /// Gets the request headers.
        /// </summary>
        public IDictionary<string, string> RequestHeaders => _httpContext.Request.Headers.ToDictionary(x => x.Key, x => string.Join(" ", x.Value.ToArray()));

        /// <summary>
        /// Gets the request method.
        /// </summary>
        public string RequestMethod => _httpContext.Request.Method;

        /// <summary>
        /// Gets the request path.
        /// </summary>
        public string RequestPath => _httpContext.Request.Path;

        /// <summary>
        /// Gets the request query string.
        /// </summary>
        public string RequestQueryString => _httpContext.Request.QueryString.ToString();

        /// <summary>
        /// Gets the user associated with the request.
        /// </summary>
        public IPrincipal User => _httpContext.User as IPrincipal;

        /// <summary>
        /// Gets the request protocol.
        /// </summary>
        public string RequestProtocol => _httpContext.Request.Protocol;

        /// <summary>
        /// Gets the request scheme.
        /// </summary>
        public string RequestScheme => _httpContext.Request.Scheme;

        /// <summary>
        /// Gets the request user agent.
        /// </summary>
        public string RequestUserAgent => _httpContext.Request.Headers["User-Agent"].ToString();

        /// <summary>
        /// Gets the response headers.
        /// </summary>
        public int ResponseStatusCode => _httpContext.Response.StatusCode;

        /// <summary>
        /// Gets the request body.
        /// </summary>
        /// <returns>The request body as a string, or null if the body cannot be read.</returns>
        public object? GetRequestBody()
        {
            if (_httpContext.Request.Body.CanSeek)
            {
                _httpContext.Request.Body.Position = 0;
                using (var reader = new StreamReader(_httpContext.Request.Body))
                {
                    var body = reader.ReadToEnd();
                    return body;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the request body.
        /// </summary>
        /// <returns></returns>
        public async Task<string?> GetRequestBodyAsync()
        {
            if (_httpContext.Request.Body.CanSeek)
            {
                _httpContext.Request.Body.Position = 0;
                using (var reader = new StreamReader(_httpContext.Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    return body;
                }
            }

            return null;
        }
    }
}
