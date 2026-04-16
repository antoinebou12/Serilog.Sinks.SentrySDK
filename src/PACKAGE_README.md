# Serilog.Sinks.SentrySDK

A Serilog sink for Sentry that simplifies error and log management in your applications. It builds on the Sentry .NET SDK and official Serilog integration.

Based on [serilog-contrib/serilog-sinks-sentry](https://github.com/serilog-contrib/serilog-sinks-sentry)

## Project status

[![.NET Core Test](https://img.shields.io/github/actions/workflow/status/antoinebou12/Serilog.Sinks.SentrySDK/tests.yml?branch=main&label=.NET%20Core%20Test&logo=github)](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/tests.yml?query=branch%3Amain)
[![.NET Core CI](https://img.shields.io/github/actions/workflow/status/antoinebou12/Serilog.Sinks.SentrySDK/CI.yml?label=.NET%20Core%20CI&logo=github)](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/CI.yml)
[![codecov](https://codecov.io/gh/antoinebou12/Serilog.Sinks.SentrySDK/branch/main/graph/badge.svg?token=DKLJUGCpI4)](https://codecov.io/gh/antoinebou12/Serilog.Sinks.SentrySDK)
[![GitHub Release](https://img.shields.io/github/v/release/antoinebou12/Serilog.Sinks.SentrySDK?logo=github&label=GitHub%20release)](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/releases/latest)

[![NuGet Serilog.Sinks.SentrySDK](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.svg?label=Serilog.Sinks.SentrySDK&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK)
[![NuGet Serilog.Sinks.SentrySDK downloads](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.svg?label=downloads&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK)
[![NuGet Serilog.Sinks.SentrySDK.AspNetCore](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.AspNetCore.svg?label=Serilog.Sinks.SentrySDK.AspNetCore&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore)
[![NuGet Serilog.Sinks.SentrySDK.AspNetCore downloads](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.AspNetCore.svg?label=downloads&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore)

## Available packages

Release **1.0.7.2** on NuGet aligns with GitHub tags such as **`v1.0.7.2`**. [GitHub Releases](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/releases) carry the changelog and attached `.nupkg` assets.

| Package | When to use |
| --- | --- |
| **Serilog.Sinks.SentrySDK** | Core sink; **net6.0** and **net10.0** in one package (`lib/net6.0/`, `lib/net10.0/`). |
| **Serilog.Sinks.SentrySDK.AspNetCore** | HTTP context integration and middleware; **net6.0** + **net10.0**. |
| **Serilog.Sinks.SentrySDK.6** / **Serilog.Sinks.SentrySDK.AspNetCore.6** | Same sink and ASP.NET Core extras; **net6.0-only** package ids. |
| **Serilog.Sinks.SentrySDK.10** / **Serilog.Sinks.SentrySDK.AspNetCore.10** | Same functionality; **net10.0-only** package ids. |

Prefer the **main** ids unless you want a single target framework per NuGet package id.

## Installation

Core sink:

```bash
dotnet add package Serilog.Sinks.SentrySDK
```

```powershell
Install-Package Serilog.Sinks.SentrySDK
```

ASP.NET Core companion (user/request context in logs):

```bash
dotnet add package Serilog.Sinks.SentrySDK.AspNetCore
```

```powershell
Install-Package Serilog.Sinks.SentrySDK.AspNetCore
```

Alternate ids (single TFM per package): **Serilog.Sinks.SentrySDK.6**, **Serilog.Sinks.SentrySDK.10**, and matching **AspNetCore** ids — see [Available packages](#available-packages).

This stack references the **Sentry** NuGet package (**6.3.2**), the same major line as [Sentry for .NET](https://docs.sentry.io/platforms/dotnet/). For DSN, `SendDefaultPii`, and debug options, see the [Quick Start](https://docs.sentry.io/platforms/dotnet/). Initialize **as early as possible** so startup failures are reported. This sink calls [`SentrySdk.Init`](https://docs.sentry.io/platforms/dotnet/configuration/options/) when you configure `WriteTo.Sentry` with a DSN; avoid a second `SentrySdk.Init` in the same process unless you deliberately use one initialization path.

Set a valid Sentry **DSN** in `appsettings.json` or in code. Without a DSN, the sink throws when the DSN argument is empty. [Demos](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/tree/main/demos) show JSON and programmatic configuration.

## Getting started

Add the sink so logs are sent to Sentry. You can use **Serilog.Settings.Configuration** with JSON:

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
          "sendDefaultPii": true,
          "environment": "Development",
          "release": "1.0.7.2",
          "attachStacktrace": true,
          "tracesSampleRate": 1.0
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
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

log.Error("This error goes to Sentry.");
```

Optional **`configureSentryOptions`** and **`beforeSend`** on `WriteTo.Sentry` map to init-time `SentryOptions` and a chained **`SetBeforeSend`**. A fuller argument list and examples are in the repository [README — Getting Started](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md#getting-started).

## Data scrubbing

Use **`SetBeforeSend`** / **`SetBeforeSendTransaction`** on [`SentryOptions`](https://docs.sentry.io/platforms/dotnet/configuration/options/) for client-side scrubbing; see [Scrubbing sensitive data](https://docs.sentry.io/platforms/dotnet/data-management/sensitive-data/). Prefer the sink’s optional **`beforeSend`** callback for event filtering; use **`configureSentryOptions`** for other flags (for example **`EnableLogs`**, **`EnableMetrics`**) and **do not** call **`SetBeforeSend`** inside that callback, because the sink registers its own `SetBeforeSend` chain. You can still use Serilog **filters** and **enrichers** to limit what reaches the sink.

## Capturing HttpContext (ASP.NET Core)

Install **Serilog.Sinks.SentrySDK.AspNetCore**, then extend the logger:

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Sentry("YOUR_SENTRY_DSN")
    .Enrich.FromLogContext()
    .Destructure.With<HttpContextDestructingPolicy>()
    .Filter.ByExcluding(e => e.Exception?.CheckIfCaptured() == true)
    .CreateLogger();
```

Register middleware (for example in `Startup` / pipeline setup):

```csharp
app.AddSentryContext();
```

Full snippets, including older `Startup.cs` examples, are in the repository [README — Capturing HttpContext](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md#capturing-httpcontext-aspnet-core).

## Sentry SDK options vs this sink

This package aligns with Sentry .NET **6.3.2**. The repository README maps **`SentryOptions`** properties and methods to **`WriteTo.Sentry`** parameters (large tables): [Sentry SDK](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md#sentry-sdk) and [Mapping to WriteTo.Sentry](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md#mapping-to-writetosentry).

Further reading: [Data Management](https://docs.sentry.io/platforms/dotnet/data-management/), [Structured logs](https://docs.sentry.io/platforms/dotnet/), [Migration guide](https://docs.sentry.io/platforms/dotnet/migration/).

## Demos, build, contributing

- Demos: [demos/README.md on GitHub](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/demos/README.md)
- Solution and local build/test commands: [README — Build, run tests, and coverage](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md#build-run-tests-and-coverage-local-development)
- [CONTRIBUTING.md](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/CONTRIBUTING.md)

Full documentation (CI, publishing, formatting): **[README.md](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md)** on GitHub.
