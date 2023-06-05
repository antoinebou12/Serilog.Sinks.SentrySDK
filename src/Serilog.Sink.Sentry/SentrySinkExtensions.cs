using System;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Sentry;

namespace Serilog
{
    public static class SentrySinkExtensions
    {
        public static LoggerConfiguration Sentry(
            this LoggerSinkConfiguration loggerConfiguration,
            string dsn,
            string release = null,
            string environment = null,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
            IFormatProvider formatProvider = null,
            string tags = null)
        {
            var options = new SentryOptions
            {
                Dsn = dsn,
                Release = release,
                Environment = environment,
            };

            var sink = new SentrySink(formatProvider, options.Dsn, options.Release, options.Environment, tags);
            return loggerConfiguration.Sink(sink, restrictedToMinimumLevel);
        }
    }
}
