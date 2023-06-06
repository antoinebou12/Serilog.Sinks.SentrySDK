using System.Security.Claims;
using Sentry;

namespace Serilog.Sinks.SentrySDK
{
    /// <summary>
    /// Defines a factory for creating Sentry users.
    /// </summary>
    public interface ISentryUserFactory
    {
        /// <summary>
        /// Creates a new Sentry user.
        /// </summary>
        /// <returns>A new <see cref="User"/> instance.</returns>
        User Create();
    }

    /// <summary>
    /// Implements <see cref="ISentryUserFactory"/> to create Sentry users
    /// based on data from the HTTP context.
    /// </summary>
    public class SentryUserFactory : ISentryUserFactory
    {
        private readonly ISentryHttpContext _httpContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentryUserFactory"/> class.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        public SentryUserFactory(ISentryHttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        /// <summary>
        /// Creates a new Sentry user based on data from the HTTP context.
        /// </summary>
        /// <returns>A new <see cref="User"/> instance.</returns>
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
