using System;
using System.IO;

using Microsoft.Extensions.Configuration;

using Sentry;

using Serilog;
using Serilog.Context;
using Serilog.Sinks.SentrySDK;
using Serilog.Exceptions;

namespace SentryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var dsn = configuration["Sentry:Dsn"]
                ?? Environment.GetEnvironmentVariable("SENTRY_DSN");

            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration);

            if (string.IsNullOrWhiteSpace(dsn))
            {
                Console.WriteLine("No Sentry DSN: set Sentry:Dsn in appsettings.json or SENTRY_DSN. Console and file sinks still run.");
            }
            else
            {
                // JSON cannot pass delegates; use configureSentryOptions / beforeSend in code (see README).
                loggerConfiguration.WriteTo.Sentry(
                    dsn: dsn,
                    environment: configuration["Sentry:Environment"],
                    serverName: "SentryConsole",
                    release: configuration["Sentry:Release"],
                    distribution: "SentryConsole",
                    tags: "app=SentryConsole",
                    operationName: "SentryConsole",
                    diagnosticLevel: "Error",
                    enableTracing: true,
                    tracesSampleRate: 1.0,
                    maxCacheItems: 100,
                    configureSentryOptions: options =>
                    {
                        options.EnableLogs = true;
                        options.EnableMetrics = true;
                    },
                    beforeSend: (sentryEvent, _) =>
                    {
                        sentryEvent.SetTag("demo", "SentryConsole");
                        return sentryEvent;
                    });
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            try
            {
                using (LogContext.PushProperty("DemoRunId", Guid.NewGuid().ToString("N")))
                {
                    var stamp = DateTime.Now.ToLongTimeString();
                    Log.Error("Intentional error logged at {Stamp}", stamp);
                    Log.Warning("Intentional warning logged at {Stamp}", stamp);
                    Log.Information("Intentional info logged at {Stamp}", stamp);
                    Log.Debug("Intentional debug logged at {Stamp}", stamp);
                    Log.Verbose("Intentional trace logged at {Stamp}", stamp);

                    ThrowParseExceptionSecondTier();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while converting to integer");
            }

            try
            {
                ThrowDivideByZeroSecondTier();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while dividing by zero");
            }

            Log.CloseAndFlush();

            Console.WriteLine("Finished");
        }

        static int ThrowDivideByZeroSecondTier() => DivByZero();

        static int DivByZero()
        {
            var i = 0;
            var j = 1 / i;
            return j;
        }

        static int ThrowParseExceptionSecondTier() => ConvertToInt();

        static int ConvertToInt()
        {
            var s = "hello world";
            _ = int.Parse(s);
            return 0;
        }
    }
}
