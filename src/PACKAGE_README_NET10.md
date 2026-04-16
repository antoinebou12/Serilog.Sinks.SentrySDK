# Serilog.Sinks.SentrySDK (.NET 10 package line)

A Serilog sink for Sentry that simplifies error and log management. It builds on the Sentry .NET SDK and official Serilog integration.

Based on [serilog-contrib/serilog-sinks-sentry](https://github.com/serilog-contrib/serilog-sinks-sentry)

[![NuGet Serilog.Sinks.SentrySDK.10](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.10.svg?label=Serilog.Sinks.SentrySDK.10&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.10)
[![NuGet Serilog.Sinks.SentrySDK.10 downloads](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.10.svg?label=downloads&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.10)
[![NuGet Serilog.Sinks.SentrySDK.AspNetCore.10](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.AspNetCore.10.svg?label=Serilog.Sinks.SentrySDK.AspNetCore.10&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore.10)
[![NuGet Serilog.Sinks.SentrySDK.AspNetCore.10 downloads](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.AspNetCore.10.svg?label=downloads&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore.10)

## This package line

Package ids **Serilog.Sinks.SentrySDK.10** and **Serilog.Sinks.SentrySDK.AspNetCore.10** ship **net10.0-only** assemblies (`lib/net10.0/`). They match the main sink and ASP.NET Core integration, under alternate ids for apps that target **.NET 10** only and want a single TFM per NuGet package.

For **net6.0 and net10.0** in one package, use [**Serilog.Sinks.SentrySDK**](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK) and [**Serilog.Sinks.SentrySDK.AspNetCore**](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore). For **net6.0-only** ids, use **Serilog.Sinks.SentrySDK.6** / **Serilog.Sinks.SentrySDK.AspNetCore.6**.

Release **1.0.7.2** on NuGet aligns with GitHub tags such as **`v1.0.7.2`**. [GitHub Releases](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/releases) carry the changelog.

## Installation

```bash
dotnet add package Serilog.Sinks.SentrySDK.10
dotnet add package Serilog.Sinks.SentrySDK.AspNetCore.10
```

```powershell
Install-Package Serilog.Sinks.SentrySDK.10
Install-Package Serilog.Sinks.SentrySDK.AspNetCore.10
```

This stack references **Sentry** (**6.3.2**). See the [Sentry .NET Quick Start](https://docs.sentry.io/platforms/dotnet/). This sink calls [`SentrySdk.Init`](https://docs.sentry.io/platforms/dotnet/configuration/options/) when you configure `WriteTo.Sentry` with a DSN; avoid duplicate in-process initialization.

## Getting started

Minimal JSON configuration (replace `YOUR_SENTRY_DSN`):

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.SentrySDK" ],
    "MinimumLevel": "Debug",
    "WriteTo": [
      {
        "Name": "Sentry",
        "Args": {
          "dsn": "YOUR_SENTRY_DSN",
          "environment": "Development",
          "release": "1.0.7.2"
        }
      }
    ],
    "Enrich": [ "FromLogContext" ]
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
```

More examples: [README — Getting Started](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md#getting-started).

## Data scrubbing

Use the sink’s optional **`beforeSend`** for event filtering and **`configureSentryOptions`** for other `SentryOptions` flags; do not call **`SetBeforeSend`** inside **`configureSentryOptions`**. See [Scrubbing sensitive data](https://docs.sentry.io/platforms/dotnet/data-management/sensitive-data/).

## Capturing HttpContext (ASP.NET Core)

With **Serilog.Sinks.SentrySDK.AspNetCore.10**:

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Sentry("YOUR_SENTRY_DSN")
    .Enrich.FromLogContext()
    .Destructure.With<HttpContextDestructingPolicy>()
    .Filter.ByExcluding(e => e.Exception?.CheckIfCaptured() == true)
    .CreateLogger();
```

```csharp
app.AddSentryContext();
```

Details: [README — Capturing HttpContext](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md#capturing-httpcontext-aspnet-core).

## Sentry SDK mapping

Property and method tables for **SentryOptions** vs **`WriteTo.Sentry`**: [README — Sentry SDK](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md#sentry-sdk).

## Demos and full documentation

- [Demos](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/tree/main/demos) and [demos/README.md](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/demos/README.md)
- [CONTRIBUTING.md](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/CONTRIBUTING.md)
- Complete guide (packages, CI, build): **[README.md](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md)**
