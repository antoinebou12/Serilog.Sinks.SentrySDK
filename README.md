# Serilog.Sink.Sentry

A Serilog sink for Sentry that simplifies error and log management in your applications.

## Project Status

[![Build status](https://ci.appveyor.com/api/projects/status/3rtn2dsk5ln6qaup?svg=true)](https://ci.appveyor.com/project/olsh/serilog-Sink-sentry)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=serilog-Sink-sentry&metric=alert_status)](https://sonarcloud.io/dashboard?id=serilog-Sink-sentry)

## Available Packages

|   | Package | Nuget |
| ------------- | ------------- | ------------- |
| Serilog.Sink.Sentry  | [Package Link](https://www.nuget.org/packages/Serilog.Sink.Sentry/) | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sink.Sentry.svg)](https://www.nuget.org/packages/Serilog.Sink.Sentry/)  |
| Serilog.Sink.Sentry.AspNetCore  | [Package Link](https://www.nuget.org/packages/Serilog.Sink.Sentry.AspNetCore/) | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sink.Sentry.AspNetCore.svg)](https://www.nuget.org/packages/Serilog.Sink.Sentry.AspNetCore/)  |

## Installation

The library is available as a [Nuget package](https://www.nuget.org/packages/Serilog.Sink.Sentry/).

You can install it with the following command:
```
Install-Package Serilog.Sink.Sentry
```

## Demos

Demos demonstrating how to use this library can be found [here](demos/).

## Getting Started

### Adding the Sentry Sink

Add the Sentry sink to your Serilog logger configuration, so that the logs will be sent to your Sentry instance. The Sentry DSN must be provided.

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Sentry("Sentry DSN")
    .Enrich.FromLogContext()
    .CreateLogger();

// By default, only messages with level errors and higher are captured
log.Error("This error goes to Sentry.");
```

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
      "Using":  [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Sentry" ],
      "MinimumLevel": "Debug",
      "WriteTo": [
        { "Name": "Console" },
        { "Name": "File", "Args": { "path": "Logs/log.txt" } },
        { "Name": "Sentry", "Args": { "dsn": "<YourSentryDsn>" } }
      ],
      "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
      "Destructure": [
        { "Name": "With", "Args": { "policy": "Sample.CustomPolicy, Sample" } },
        { "Name": "ToMaximumDepth", "Args": { "maximumDestructuringDepth": 4 } },
        { "Name": "ToMaximumStringLength", "Args": { "maximumStringLength": 100 } },
        { "Name": "ToMaximumCollectionCount", "Args": { "maximumCollectionCount": 10 } }
      ],
      "Properties": {
          "Application": "Sample"
      }
  }


}
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

First, install the [ASP.NET Core sink](https://www.nuget.org/packages/Serilog.Sink.Sentry.AspNetCore/) with the command:

```
Install-Package Serilog.Sink.Sentry.AspNetCore
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
