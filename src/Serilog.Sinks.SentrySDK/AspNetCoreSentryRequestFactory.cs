using System.Collections.Generic;
using Sentry;

namespace Serilog.Sinks.SentrySDK
{
    /// <summary>
    /// Defines a factory for creating Sentry requests.
    /// </summary>
    public interface ISentryRequestFactory
    {
        /// <summary>
        /// Creates a new Sentry request.
        /// </summary>
        /// <returns>A new instance of an object implementing <see cref="ISentryRequest"/>.</returns>
        ISentryRequest Create();
    }

    /// <summary>
    /// Provides an implementation for <see cref="ISentryRequestFactory"/>, creating Sentry requests based on the HTTP context.
    /// </summary>
    public class SentryRequestFactory : ISentryRequestFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SentryRequestFactory"/> class.
        /// </summary>
        /// <param name="sentryHttpContext">The HTTP context.</param>
        public SentryRequestFactory(ISentryHttpContext sentryHttpContext)
        {
            SentryHttpContext = sentryHttpContext;
        }

        private ISentryHttpContext SentryHttpContext { get; }

        /// <summary>
        /// Creates a new Sentry request based on the current HTTP context.
        /// </summary>
        /// <returns>A new <see cref="ISentryRequest"/> instance.</returns>
        public ISentryRequest Create()
        {
            var request = new SentryRequest
            {
                Url = SentryHttpContext.RequestPath,
                Method = SentryHttpContext.RequestMethod,
                Environment = new Dictionary<string, string>(),
                Headers = SentryHttpContext.RequestHeaders,
                Cookies = SentryHttpContext.RequestCookies,
                Data = SentryHttpContext.GetRequestBody() as string,
                QueryString = SentryHttpContext.RequestQueryString
            };

            return request;
        }
    }

    /// <summary>
    /// Defines a Sentry request.
    /// </summary>
    public interface ISentryRequest
    {
        /// <inheritdoc/>
        string Url { get; set; }
        /// <inheritdoc/>
        string Method { get; set; }
        /// <inheritdoc/>
        IDictionary<string, string> Environment { get; set; }
        /// <inheritdoc/>
        IDictionary<string, string> Headers { get; set; }
        /// <inheritdoc/>
        IDictionary<string, string> Cookies { get; set; }
        /// <inheritdoc/>
        string Data { get; set; }
        /// <inheritdoc/>
        string QueryString { get; set; }
    }

    /// <summary>
    /// Represents a Sentry request, including details such as URL, method, headers, and more.
    /// </summary>
    public class SentryRequest : ISentryRequest
    {
        /// <inheritdoc/>
        public string Url { get; set; }
        /// <inheritdoc/>
        public string Method { get; set; }
        /// <inheritdoc/>
        public IDictionary<string, string> Environment { get; set; }
        /// <inheritdoc/>
        public IDictionary<string, string> Headers { get; set; }
        /// <inheritdoc/>
        public IDictionary<string, string> Cookies { get; set; }
        /// <inheritdoc/>
        public string Data { get; set; }
        /// <inheritdoc/>
        public string QueryString { get; set; }
    }
}