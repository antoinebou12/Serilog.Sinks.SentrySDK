using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Serilog.Sinks.SentrySDK;
namespace Serilog.Sinks.SentrySDK.AspNetCore
{
    public class AspCoreHttpContextAdapter : ISentryHttpContext
    {
        private readonly HttpContext _httpContext;

        public AspCoreHttpContextAdapter(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public string RemoteIpAddress => _httpContext.Connection.RemoteIpAddress.ToString();

        public IDictionary<string, string> RequestCookies =>
            _httpContext.Request.Cookies.ToDictionary(x => x.Key, x => x.Value);

        public IDictionary<string, string> RequestHeaders => _httpContext.Request.Headers.ToDictionary(x => x.Key, x => string.Join(" ", x.Value.ToArray()));

        public string RequestMethod => _httpContext.Request.Method;

        public string RequestPath => _httpContext.Request.Path;

        public string RequestQueryString => _httpContext.Request.QueryString.ToString();

        public IPrincipal User => _httpContext.User as IPrincipal;

        public object GetRequestBody()
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
    }
}
