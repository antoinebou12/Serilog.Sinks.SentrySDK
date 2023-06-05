using System.Collections.Generic;

using Sentry;

namespace Serilog.Sinks.SentrySDK
{
    public interface ISentryRequestFactory
    {
        ISentryRequest Create();
    }

    public class SentryRequestFactory : ISentryRequestFactory
    {
        public SentryRequestFactory(ISentryHttpContext sentryHttpContext)
        {
            SentryHttpContext = sentryHttpContext;
        }

        private ISentryHttpContext SentryHttpContext { get; }

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

    public interface ISentryRequest
    {
        string Url { get; set; }
        string Method { get; set; }
        IDictionary<string, string> Environment { get; set; }
        IDictionary<string, string> Headers { get; set; }
        IDictionary<string, string> Cookies { get; set; }
        string Data { get; set; }
        string QueryString { get; set; }
    }

    public class SentryRequest : ISentryRequest
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public IDictionary<string, string> Environment { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public IDictionary<string, string> Cookies { get; set; }
        public string Data { get; set; }
        public string QueryString { get; set; }
    }
}
