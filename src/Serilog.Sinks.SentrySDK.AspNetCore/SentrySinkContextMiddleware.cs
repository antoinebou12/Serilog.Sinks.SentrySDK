using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Serilog.Sinks.SentrySDK.AspNetCore
{
    /// <summary>
    /// Middleware for capturing Sentry context in Serilog for ASP.NET Core applications.
    /// </summary>
    public class SentrySinkContextMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentrySinkContextMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public SentrySinkContextMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty(SentrySinkConstants.HttpContextKey, new AspCoreHttpContextAdapter(context), true))
            {
                try
                {
                    await _next(context);
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "Connection id \"{TraceIdentifier}\": An unhandled exception was thrown by the application.", context.TraceIdentifier);
                    throw;
                }
            }
        }
    }
}
