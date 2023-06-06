using System;
using System.IO;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Events;
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
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                // Explicitly call our error logger
                var errorLog = "Intentional error logged at " + DateTime.Now.ToLongTimeString();
                Log.Error(errorLog);

                var warningLog = "Intentional warning logged at " + DateTime.Now.ToLongTimeString();
                Log.Warning(errorLog);

                var infoLog = "Intentional info logged at " + DateTime.Now.ToLongTimeString();
                Log.Information(errorLog);

                var debugLog = "Intentional debug logged at " + DateTime.Now.ToLongTimeString();
                Log.Debug(errorLog);

                var traceLog = "Intentional trace logged at " + DateTime.Now.ToLongTimeString();
                Log.Verbose(errorLog);

                // Trigger an exception
                ConvertToIntSecondTier();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while converting to integer");
            }

            try
            {
                // Trigger another exception
                DivByZeroSecondTier();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while dividing by zero");
            }

            Log.CloseAndFlush();

            Console.WriteLine("Finished");
        }

        static int DivByZeroSecondTier()
        {
            var i = DivByZero();
            return i;
        }

        static int DivByZero()
        {
            var i = 0;

            if (i == 0)
            {
                // Handle the error, maybe by returning a default value or logging the error
                Log.Error("Attempted division by zero");
                return 0;
            }

            var j = 1 / i;
            return j;
        }

        static int ConvertToIntSecondTier()
        {
            var i = ConvertToInt();
            return i;
        }

        static int ConvertToInt()
        {
            var s = "hello world";

            if (!int.TryParse(s, out int result))
            {
                // Handle the error, maybe by returning a default value or logging the error
                Log.Error("Failed to convert string to integer");
                return 0;
            }

            return result;
        }

    }
}
