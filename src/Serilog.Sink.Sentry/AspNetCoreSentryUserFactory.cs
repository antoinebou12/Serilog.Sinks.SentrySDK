using System.Security.Claims;
using Sentry;

namespace Serilog
{
    public interface ISentryUserFactory
    {
        User Create();
    }

    public class SentryUserFactory : ISentryUserFactory
    {
        private readonly ISentryHttpContext _httpContext;

        public SentryUserFactory(ISentryHttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public User Create()
        {
            var claimsIdentity = _httpContext.User.Identity as ClaimsIdentity;
            var sentryUser = new User
            {
                IpAddress = _httpContext.RemoteIpAddress,
                // Suppose you store username and email in claims.
                Username = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value,
                Email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value
            };

            return sentryUser;
        }
    }
}
