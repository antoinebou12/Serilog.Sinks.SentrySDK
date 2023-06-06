using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace Serilog.Sinks.SentrySDK.AspNetCore
{
    /// <summary>
    /// Contains extension methods for an application.
    /// </summary>
    public static class SentrySinkContextMiddlewareExtensions
    {
        /// <summary>
        /// Adds Sentry context middleware to the app.
        /// This middleware enables buffering and uses the SentrySinkContextMiddleware.
        /// </summary>
        /// <param name="app">The IApplicationBuilder instance this method extends.</param>
        /// <returns>The IApplicationBuilder for chaining additional methods.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the app is null.</exception>
        public static IApplicationBuilder AddSentryContext(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.Use(next => context =>
            {
                try
                {
                    context.Request.EnableBuffering();
                    return next(context);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while enabling buffering.");
                    throw;
                }
            });

            try
            {
                return app.UseMiddleware<SentrySinkContextMiddleware>();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while adding Sentry context middleware.");
                throw;
            }
        }
    }
}
