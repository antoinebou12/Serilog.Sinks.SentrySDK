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
          "active": true,
          "includeActivityData": true,
          "sendDefaultPii": true,
          "maxBreadcrumbs": 200,
          "maxQueueItems": 100,
          "debug": true,
          "diagnosticLevel": "Error",
          "environment": "Development",
          "operationName": "SentryConsole",
          "release": "1.0.3",
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
