# Serilog.Sinks.SentrySDK

A Serilog sink for Sentry that simplifies error and log management in your applications.

Based on [serilog-contrib/serilog-sinks-sentry](https://github.com/serilog-contrib/serilog-sinks-sentry)

## Project Status

[![.NET Core Test](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/tests.yml/badge.svg)](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/tests.yml)
[![.NET Core CI](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/CI.yml/badge.svg)](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/CI.yml)
[![CodeQL](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/codeql.yml/badge.svg)](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/codeql.yml)
[![codecov](https://codecov.io/gh/antoinebou12/Serilog.Sinks.SentrySDK/branch/main/graph/badge.svg?token=DKLJUGCpI4)](https://codecov.io/gh/antoinebou12/Serilog.Sinks.SentrySDK)

## Available Packages

|                                    | Package                                                                            | Nuget                                                                                                                                                 |
| ---------------------------------- | ---------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| Serilog.Sinks.SentrySDK            | [Package Link](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.6/)            | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.6.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.6/)                       |
| Serilog.Sinks.SentrySDK.AspNetCore | [Package Link](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore.6/) | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.AspNetCore.6.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore.6/) |


> Q1 2024
> ![serilog](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/assets/13888068/9bd394c0-0c78-4676-93ca-a7594fbeb537)

> I moving the package to my personal account
> ## Old package

|                                    | Package                                                                            | Nuget                                                                                                                                                 |
| ---------------------------------- | ---------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| Serilog.Sinks.SentrySDK            | [Package Link](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK/)            | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK/)                       |
| Serilog.Sinks.SentrySDK.AspNetCore | [Package Link](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore/) | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.AspNetCore.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore/) |


## Installation

The library is available as a [Nuget package](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK/).

You can install it with the following command:

```
dotnet add package Serilog.Sinks.SentrySDK
Install-Package Serilog.Sinks.SentrySDK
```

## Demos

Demos demonstrating how to use this library can be found [here](demos/).

## Getting Started

### Adding the Sentry Sink

Add the Sentry sink to your Serilog logger configuration, so that the logs will be sent to your Sentry instance. The Sentry DSN must be provided.

You can also configure Serilog using a JSON configuration. Here's a sample:

```json
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "Serilog": {
    "Using": [
      "Serilog.Sinks.SentrySDK"
    ],
    "MinimumLevel": "Debug",
    "WriteTo": [
      {
        "Name": "Sentry",
        "Args": {
          "dsn": "",
          "sendDefaultPii": true,
          "maxBreadcrumbs": 200,
          "maxQueueItems": 100,
          "debug": true,
          "diagnosticLevel": "Error",
          "environment": "Development",
          "operationName": "SentryConsole",
          "release": "1.0.5",
          "serverName": "SentryConsole",
          "dist": "SentryConsole",
          "tags": "SentryConsole=SentryConsole",
          "tracesSampleRate": 1.0,
          "tracesSampler": "AlwaysSample",
          "stackTraceMode": "Enhanced",
          "isGlobalModeEnabled": true,
          "sampleRate": 1.0,
          "attachStacktrace": true,
          "autoSessionTracking": true,
          "enableTracing": true
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"],
    "Properties": {
      "Application": "Sample"
    }
  }
}
```

```csharp
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

    var log = new LoggerConfiguration()
              .ReadFrom.Configuration(configuration)
              .Enrich.FromLogContext()
              .CreateLogger();

// By default, only messages with level errors and higher are captured
log.Error("This error goes to Sentry.");
```

### Data Scrubbing

Data scrubbing allows you to sanitize your logs before they are sent to Sentry. This can be useful for removing sensitive information.

To use it, provide a custom `IScrubber` implementation when setting up the Sentry Sink:

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Sentry("Sentry DSN", dataScrubber: new MyDataScrubber())
    .Enrich.FromLogContext()
    .CreateLogger();
```

### Capturing HttpContext (ASP.NET Core)

To include user, request body, and header information in the logs, some additional setup is required.

First, install the [ASP.NET Core sink](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore/) with the command:

```
dotnet add package Serilog.Sinks.SentrySDK.AspNetCore
Install-Package Serilog.Sinks.SentrySDK.AspNetCore
```

Then, update your logger configuration to include a custom `HttpContextDestructingPolicy`:

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Sentry("Sentry DSN")
    .Enrich.FromLogContext()

    // Add this two lines to the logger configuration
    .Destructure.With<HttpContextDestructingPolicy>()
    .Filter.ByExcluding(e => e.Exception?.CheckIfCaptured() == true)

    .CreateLogger();
```

Finally, add the Sentry context middleware to your `Startup.cs`:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    // Add this line
    app.AddSentryContext();

    // Other stuff
}
```

With these steps, your logs will include detailed information about the HTTP context of the requests.


## Sentry SDK
### Properties
* `BackgroundWorker`: A property that gets or sets the worker used by the client to pass envelopes.
* `SentryScopeStateProcessor`: A property to get or set the Scope state processor.
* `SendDefaultPii`: A property to get or set whether to include default Personal Identifiable Information.
* `NetworkStatusListener`: A property to get or set a mechanism to convey network status to the caching transport.
* `ServerName`: A property to get or set the name of the server running the application.
* `AttachStacktrace`: A property to get or set whether to send the stack trace of an event captured without an exception.
* `IsEnvironmentUser`: A property to get or set whether to report the System.Environment.UserName as the User affected in the event.
* `SampleRate`: A property to get or set the optional sample rate.
* `ShutdownTimeout`: A property to get or set how long to wait for events to be sent before shutdown.
* `MaxBreadcrumbs`: A property to get or set the maximum breadcrumbs.
* `MaxQueueItems`: A property to get or set the maximum number of events to keep while the worker attempts to send them.
* `BeforeBreadcrumb`: A property to get or set a callback function to be invoked when a breadcrumb is about to be stored.
* `BeforeSendTransaction`: A property to get or set a callback to invoke before sending a transaction to Sentry.
* `MaxCacheItems`: A property to get or set the maximum number of events to keep in cache.
* `Dsn`: A property to get or set the Data Source Name of a given project in Sentry.
* `Environment`: A property to get or set the environment the application is running.
* `Distribution`: A property to get or set the distribution of the application, associated with the release set in `SentryOptions.Release`.
* `Release`: A property to get or set the release information for the application.
* `BeforeSend`: A property to get or set a callback to invoke before sending an event to Sentry.

### Methods
* `AddJsonConverter(JsonConverter converter)`: A method to add a `JsonConverter` to be used when serializing or deserializing objects to JSON with the SDK.
* `SetBeforeBreadcrumb(Func<Breadcrumb, Breadcrumb?> beforeBreadcrumb)`: A method to set a callback function to be invoked when a breadcrumb is about to be stored.
* `SetBeforeBreadcrumb(Func<Breadcrumb, Hint, Breadcrumb?> beforeBreadcrumb)`: Another overload of `SetBeforeBreadcrumb` method that accepts a `Hint`.
* `SetBeforeSend(Func<SentryEvent, SentryEvent?> beforeSend)`: A method to configure a callback function to be invoked before sending an event to Sentry.
* `SetBeforeSend(Func<SentryEvent, Hint, SentryEvent?> beforeSend)`: Another overload of `SetBeforeSend` method that accepts a `Hint`.
* `SetBeforeSendTransaction(Func<Transaction, Transaction?> beforeSendTransaction)`: A method to configure a callback to invoke before sending a transaction to Sentry.
* `SetBeforeSendTransaction(Func<Transaction, Hint, Transaction?> beforeSendTransaction)`: Another overload of `SetBeforeSendTransaction` method that accepts a `Hint`.
