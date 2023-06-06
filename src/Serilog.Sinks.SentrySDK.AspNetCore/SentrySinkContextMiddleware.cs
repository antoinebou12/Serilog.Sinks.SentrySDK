using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Serilog.Sinks.SentrySDK.AspNetCore
{
    internal class SentrySinkContextMiddleware
    {
        private readonly RequestDelegate _next;

        public SentrySinkContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

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
