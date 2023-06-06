### SentryConsole
This project demonstrates the simplest use-case of Serilog with Sentry in a .NET Core Console Application. Here, Sentry is used as a sink for Serilog, capturing exceptions and log messages, and sending them to the Sentry service for error tracking.

To run this demo:
1. Replace "Sentry DSN" with your actual Sentry DSN in the `dsn:"<dsn>"` appsettings.json.
2. Run the application.
3. Observe that log messages and exceptions are sent to Sentry, which can be viewed in your Sentry dashboard.

The project aims to showcase how to instrument a console application with minimal configuration.

### SentryWeb
This project demonstrates how to use Serilog and Sentry in a .NET Core MVC Web Application. It extends the functionality shown in the SentryConsole application, showcasing how to track exceptions and logs from a web-based application.

To run this demo:
1. Replace "Sentry DSN" with your actual Sentry DSN in the `dsn:"<dsn>"` appsettings.json.
2. Run the application and interact with the web interface to generate log messages and trigger exceptions.
3. Observe that log messages, exceptions, and HTTP request information are sent to Sentry, which can be viewed in your Sentry dashboard.

This project aims to demonstrate how to use Serilog and Sentry to track errors in a more complex application that includes user interactions and web requests.
