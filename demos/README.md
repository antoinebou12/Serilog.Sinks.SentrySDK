### SentryConsole
This project demonstrates the simplest use-case of Serilog with Sentry in a .NET Core Console Application. Here, Sentry is used as a sink for Serilog, capturing exceptions and log messages, and sending them to the Sentry service for error tracking.

To run this demo:
1. Replace the placeholder DSN with your actual Sentry DSN in `SentryConsole/appsettings.json` (`Serilog:WriteTo` → `Args:dsn`).
2. From the repository root: `dotnet run --project demos/SentryConsole/SentryConsole.csproj`
3. Observe that log messages and exceptions are sent to Sentry, which can be viewed in your Sentry dashboard.

The project aims to showcase how to instrument a console application with minimal configuration.

### SentryWeb
This project demonstrates how to use Serilog and Sentry in a .NET Core MVC Web Application. It extends the functionality shown in the SentryConsole application, showcasing how to track exceptions and logs from a web-based application.

To run this demo:
1. Replace the placeholder DSN with your actual Sentry DSN in `SentryWeb/appsettings.json` (`Serilog:WriteTo` → `Args:dsn`).
2. From the repository root: `dotnet run --project demos/SentryWeb/SentryWeb.csproj`, then use the site to generate logs and exceptions.
3. Observe that log messages, exceptions, and HTTP request information are sent to Sentry, which can be viewed in your Sentry dashboard.

This project aims to demonstrate how to use Serilog and Sentry to track errors in a more complex application that includes user interactions and web requests.
