using System.Collections.Generic;
using System.Security.Principal;

namespace Serilog.Sinks.SentrySDK
{
    /// <summary>
    /// Defines an interface for providing HTTP context data to Sentry.
    /// </summary>
    public interface ISentryHttpContext
    {
        /// <summary>
        /// Gets the remote IP address of the request.
        /// </summary>
        string RemoteIpAddress { get; }

        /// <summary>
        /// Gets a dictionary of the cookies from the request.
        /// </summary>
        IDictionary<string, string> RequestCookies { get; }

        /// <summary>
        /// Gets a dictionary of the headers from the request.
        /// </summary>
        IDictionary<string, string> RequestHeaders { get; }

        /// <summary>
        /// Gets the method of the request.
        /// </summary>
        string RequestMethod { get; }

        /// <summary>
        /// Gets the path of the request.
        /// </summary>
        string RequestPath { get; }

        /// <summary>
        /// Gets the query string of the request.
        /// </summary>
        string RequestQueryString { get; }

        /// <summary>
        /// Gets the user principal of the request.
        /// </summary>
        IPrincipal User { get; }

        /// <summary>
        /// Retrieves the body of the request.
        /// </summary>
        /// <returns>The request body as an object.</returns>
        object GetRequestBody();
    }
}
