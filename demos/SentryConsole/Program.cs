using System;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;

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
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                // Explicitly call our error logger
                Log.Error("Intentional error logged at {TimeStamp}", DateTime.Now.ToLongTimeString());

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
            return Convert.ToInt32(s);
        }
    }
}
