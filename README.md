# Serilog.Sinks.SentrySDK

A Serilog sink for Sentry that simplifies error and log management in your applications.

Based on [serilog-contrib/serilog-sinks-sentry](https://github.com/serilog-contrib/serilog-sinks-sentry)

## Project Status

Workflows (click a badge to open the run on GitHub Actions):

[![.NET Core Test](https://img.shields.io/github/actions/workflow/status/antoinebou12/Serilog.Sinks.SentrySDK/tests.yml?branch=main&label=.NET%20Core%20Test&logo=github)](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/tests.yml?query=branch%3Amain)
[![.NET Core CI](https://img.shields.io/github/actions/workflow/status/antoinebou12/Serilog.Sinks.SentrySDK/CI.yml?label=.NET%20Core%20CI&logo=github)](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/CI.yml)
[![CodeQL](https://img.shields.io/github/actions/workflow/status/antoinebou12/Serilog.Sinks.SentrySDK/codeql.yml?branch=main&label=CodeQL&logo=github)](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/actions/workflows/codeql.yml?query=branch%3Amain)
[![codecov](https://codecov.io/gh/antoinebou12/Serilog.Sinks.SentrySDK/branch/main/graph/badge.svg?token=DKLJUGCpI4)](https://codecov.io/gh/antoinebou12/Serilog.Sinks.SentrySDK)
[![GitHub Release](https://img.shields.io/github/v/release/antoinebou12/Serilog.Sinks.SentrySDK?logo=github&label=GitHub%20release)](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/releases/latest)

NuGet.org all-time downloads (shields.io):

[![Serilog.Sinks.SentrySDK](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.svg?label=Serilog.Sinks.SentrySDK&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK)
[![Serilog.Sinks.SentrySDK.AspNetCore](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.AspNetCore.svg?label=Serilog.Sinks.SentrySDK.AspNetCore&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore)
[![Serilog.Sinks.SentrySDK.6](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.6.svg?label=Serilog.Sinks.SentrySDK.6&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.6)
[![Serilog.Sinks.SentrySDK.AspNetCore.6](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.AspNetCore.6.svg?label=Serilog.Sinks.SentrySDK.AspNetCore.6&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore.6)

All workflow files live under [`.github/workflows`](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/tree/main/.github/workflows).

## Available Packages

NuGet.org listings (this repo’s CI publishes the **non-`.6`** ids; **`.6`** ids are the same product line with an alternate package name on NuGet):

| Package | NuGet.org | Version | Downloads |
| --- | --- | --- | --- |
| Serilog.Sinks.SentrySDK | [Serilog.Sinks.SentrySDK](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK) | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK) | [![NuGet](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK) |
| Serilog.Sinks.SentrySDK.AspNetCore | [Serilog.Sinks.SentrySDK.AspNetCore](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore) | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.AspNetCore.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore) | [![NuGet](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.AspNetCore.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore) |
| Serilog.Sinks.SentrySDK.6 | [Serilog.Sinks.SentrySDK.6](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.6) | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.6.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.6) | [![NuGet](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.6.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.6) |
| Serilog.Sinks.SentrySDK.AspNetCore.6 | [Serilog.Sinks.SentrySDK.AspNetCore.6](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore.6) | [![NuGet](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.AspNetCore.6.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore.6) | [![NuGet](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.AspNetCore.6.svg)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore.6) |

**Releases:** [GitHub Releases](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/releases) (changelog and `.nupkg` assets attached to published releases).

**Release 1.0.7.2** (see `VersionPrefix` in the `.csproj` files): NuGet packages ship **both** `net6.0` and `net10.0` in the same `.nupkg` (`lib/net6.0/`, `lib/net10.0/`). Use a matching GitHub release tag such as **`v1.0.7.2`**. **Publishing to [nuget.org](https://www.nuget.org/)** runs automatically when you **[publish a GitHub Release](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/releases/new)** (see [.github/workflows/CI.yml](.github/workflows/CI.yml): pack, then push with `NUGET_API_KEY`). Manual [workflow_dispatch](https://docs.github.com/en/actions/using-workflows/manually-running-a-workflow) builds and uploads artifacts only; it does **not** push to NuGet.

## Installation

The library is available on NuGet as [Serilog.Sinks.SentrySDK](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK) (or [Serilog.Sinks.SentrySDK.6](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.6) if you prefer that package id).

You can install it with the following command:

```
dotnet add package Serilog.Sinks.SentrySDK
Install-Package Serilog.Sinks.SentrySDK
```

This package references the **Sentry** NuGet package (**6.3.2**), the same major line as the [Sentry for .NET](https://docs.sentry.io/platforms/dotnet/) documentation. For generic SDK setup (DSN, `SendDefaultPii`, debug), see the official [Quick Start](https://docs.sentry.io/platforms/dotnet/). Initialize the SDK **as early as possible** so startup failures are reported. This sink calls [`SentrySdk.Init`](https://docs.sentry.io/platforms/dotnet/configuration/options/) internally when you configure `WriteTo.Sentry` with a DSN; avoid calling `SentrySdk.Init` again for the same process unless you coordinate a single initialization path.

## Demos

Demos demonstrating how to use this library can be found [here](demos/) and in [demos/README.md](demos/README.md).

## Build, run tests, and coverage (local development)

**Prerequisites:** [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) to build and run tests. The published libraries **multi-target** `net6.0` and `net10.0` (one package includes both under `lib/`). CI also installs the **6.0.x** SDK band so `net6.0` targets build cleanly on the runner.

From the repository root:

```bash
# Restore and build the main library
dotnet restore src/Serilog.Sinks.SentrySDK/Serilog.Sinks.SentrySDK.csproj
dotnet build src/Serilog.Sinks.SentrySDK/Serilog.Sinks.SentrySDK.csproj -c Release

# Build the ASP.NET Core companion package
dotnet build src/Serilog.Sinks.SentrySDK.AspNetCore/Serilog.Sinks.SentrySDK.AspNetCore.csproj -c Release

# Run all unit tests (xUnit)
dotnet test src/Serilog.Sinks.SentrySDK.Tests/Serilog.Sinks.SentrySDK.Tests.csproj
dotnet test src/Serilog.Sinks.SentrySDK.AspNetCore.Tests/Serilog.Sinks.SentrySDK.AspNetCore.Tests.csproj
```

Run tests with coverage (same approach as CI, using [coverlet](https://github.com/coverlet-coverage/coverlet)):

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./reports/coverage src/Serilog.Sinks.SentrySDK.Tests/Serilog.Sinks.SentrySDK.Tests.csproj
```

CI uploads Cobertura to [Codecov](https://codecov.io/gh/antoinebou12/Serilog.Sinks.SentrySDK); keep or improve coverage when you change production code.

### Code formatting (dotnet format)

The repo includes an [`.editorconfig`](.editorconfig) (UTF-8, LF, 4-space indent for C#). CI runs `dotnet format` with `--verify-no-changes` on [`src/Serilog.Sinks.SentrySDK.sln`](src/Serilog.Sinks.SentrySDK.sln).

Apply formatting locally (same as fixing style before a PR):

```bash
dotnet restore src/Serilog.Sinks.SentrySDK.sln
dotnet format src/Serilog.Sinks.SentrySDK.sln
```

Check only (no file writes; fails if the tree does not match the formatter):

```bash
dotnet format src/Serilog.Sinks.SentrySDK.sln --verify-no-changes
```

**Configure the sink:** set a valid Sentry **DSN** (from your Sentry project settings) in `appsettings.json` or in code. Demos under `demos/` show JSON and programmatic configuration. Without a DSN, the sink constructor throws when the DSN argument is empty.

**Contributing:** see [CONTRIBUTING.md](CONTRIBUTING.md) (pull requests, issues, style, and tests).

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
          "release": "1.0.7.2",
          "serverName": "SentryConsole",
          "distribution": "SentryConsole",
          "tags": "SentryConsole=SentryConsole",
          "tracesSampleRate": 1.0,
          "stackTraceMode": "Enhanced",
          "sampleRate": 1.0,
          "attachStacktrace": true,
          "autoSessionTracking": true,
          "enableTracing": true,
          "isEnvironmentUser": true,
          "shutdownTimeout": 2.0,
          "maxCacheItems": 30
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

Optional **`configureSentryOptions`** and **`beforeSend`** parameters on `WriteTo.Sentry` map to init-time `SentryOptions` and a chained **`SetBeforeSend`** (see [Data scrubbing](#data-scrubbing) and the [Methods](#methods) table).

### Data scrubbing

Client-side scrubbing of events is done with **`SetBeforeSend`** / **`SetBeforeSendTransaction`** on [`SentryOptions`](https://docs.sentry.io/platforms/dotnet/configuration/options/) at init time; see [Scrubbing sensitive data](https://docs.sentry.io/platforms/dotnet/data-management/sensitive-data/) in the Sentry .NET docs. You can also use **server-side** scrubbing in your Sentry project settings so data is not stored.

The `WriteTo.Sentry` overload supports an optional **`beforeSend`** callback (chained after the sink’s internal `EventId` handling). Use optional **`configureSentryOptions`** for other flags (for example **`EnableLogs`**, **`EnableMetrics`**, **`SetBeforeSendMetric`**) without calling **`SetBeforeSend`** in that callback, because the sink registers its own `SetBeforeSend` chain—use **`beforeSend`** for event filtering instead.

For background on what the SDK may collect, see [Data collected](https://docs.sentry.io/platforms/dotnet/data-management/data-collected/) and the [Data Management](https://docs.sentry.io/platforms/dotnet/data-management/) hub.

You can still use Serilog **filters** and **enrichers** to limit what reaches the sink.

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

Reference for the Sentry .NET SDK [`SentryOptions`](https://docs.sentry.io/platforms/dotnet/configuration/options/) (this package references Sentry **6.3.2**). The **This sink** column describes how each member relates to `WriteTo.Sentry` in this library.

### Properties

| Property | Description | This sink |
| --- | --- | --- |
| `BackgroundWorker` | Gets or sets the worker used by the client to pass envelopes. | Not exposed |
| `SentryScopeStateProcessor` | Gets or sets the scope state processor. | Not exposed |
| `SendDefaultPii` | Gets or sets whether to include default personally identifiable information. | Yes — `sendDefaultPii` |
| `NetworkStatusListener` | Gets or sets a mechanism to convey network status to the caching transport. | Not exposed |
| `ServerName` | Gets or sets the name of the server running the application. | Yes — `serverName` (if omitted, the sink defaults to the machine name) |
| `AttachStacktrace` | Gets or sets whether to send the stack trace of an event captured without an exception. | Yes — `attachStacktrace` |
| `IsEnvironmentUser` | Gets or sets whether to report `System.Environment.UserName` as the user affected in the event. | Yes — `isEnvironmentUser` |
| `SampleRate` | Gets or sets the optional sample rate for **error events**. | Yes — `sampleRate` |
| `ShutdownTimeout` | Gets or sets how long to wait for events to be sent before shutdown. | Yes — `shutdownTimeout` (seconds) |
| `MaxBreadcrumbs` | Gets or sets the maximum breadcrumbs. | Yes — `maxBreadcrumbs` |
| `MaxQueueItems` | Gets or sets the maximum number of events to keep while the worker attempts to send them. | Yes — `maxQueueItems` |
| `BeforeBreadcrumb` | Gets or sets a callback invoked when a breadcrumb is about to be stored. | Not exposed |
| `BeforeSendTransaction` | Gets or sets a callback invoked before sending a transaction to Sentry. | Not exposed |
| `MaxCacheItems` | Gets or sets the maximum number of events to keep in cache. | Yes — `maxCacheItems` |
| `Dsn` | Gets or sets the Data Source Name of a given project in Sentry. | Yes — `dsn` |
| `Environment` | Gets or sets the environment the application is running in. | Yes — `environment` |
| `Distribution` | Gets or sets the distribution of the application, associated with the release set in `SentryOptions.Release`. | Yes — `distribution` |
| `Release` | Gets or sets the release information for the application. | Yes — `release` |
| `BeforeSend` | Gets or sets a callback invoked before sending an event to Sentry. | Chained — optional `beforeSend` on `WriteTo.Sentry` runs after the sink’s `EventId` mapping; optional `configureSentryOptions` for other options (do not call `SetBeforeSend` there) |
| `Debug` | Enables SDK debug mode (verbose client logging). | Yes — `debug` |
| `DiagnosticLevel` | Minimum level for SDK diagnostic log output when `Debug` is enabled. | Yes — `diagnosticLevel` |
| `TracesSampleRate` | Sample rate for **performance monitoring** (transactions/spans); separate from error `SampleRate`. | Yes — `tracesSampleRate` |
| `AutoSessionTracking` | Enables automatic session tracking. | Yes — `autoSessionTracking` |
| `StackTraceMode` | Stack trace capture mode (for example `Original` or `Enhanced`). | Yes — `stackTraceMode` |

### Methods

| Method | Description | This sink |
| --- | --- | --- |
| `AddJsonConverter(JsonConverter converter)` | Adds a `JsonConverter` used when serializing or deserializing JSON in the SDK. | Not exposed |
| `SetBeforeBreadcrumb(Func<Breadcrumb, Breadcrumb?> beforeBreadcrumb)` | Sets a callback when a breadcrumb is about to be stored. | Not exposed |
| `SetBeforeBreadcrumb(Func<Breadcrumb, SentryHint, Breadcrumb?> beforeBreadcrumb)` | Overload of `SetBeforeBreadcrumb` that accepts a `SentryHint`. | Not exposed |
| `SetBeforeSend(Func<SentryEvent, SentryEvent?> beforeSend)` | Configures a callback before sending an error event. | Use optional `beforeSend` on `WriteTo.Sentry` (`SentryHint` overload not separately exposed) |
| `SetBeforeSend(Func<SentryEvent, SentryHint, SentryEvent?> beforeSend)` | Overload of `SetBeforeSend` that accepts a `SentryHint`. | Optional `beforeSend` parameter (`Func<SentryEvent, SentryHint, SentryEvent?>`) |
| `SetBeforeSendTransaction(Func<Transaction, Transaction?> beforeSendTransaction)` | Configures a callback before sending a transaction. | Not exposed |
| `SetBeforeSendTransaction(Func<Transaction, SentryHint, Transaction?> beforeSendTransaction)` | Overload of `SetBeforeSendTransaction` that accepts a `SentryHint`. | Not exposed |

### Mapping to `WriteTo.Sentry`

These parameters are part of the Serilog configuration API but are not direct `SentryOptions` property names:

| Parameter | Role |
| --- | --- |
| `formatProvider` | Culture-specific formatting for rendered log properties. |
| `restrictedToMinimumLevel` | Minimum Serilog level forwarded to the sink. |
| `tags` | Comma-separated `key=value` pairs applied on the Sentry scope. |
| `transactionName`, `operationName` | Used for scope tags and transaction naming in the sink. |
| `transactionService` | Optional `ITransactionService` for span creation (dependency injection). |
| `enableTracing` | When `false`, sets `TracesSampleRate` to `0` and skips per-log transactions/spans in the sink. When `true`, uses `tracesSampleRate` for performance monitoring. |
| `configureSentryOptions` | Optional `Action<SentryOptions>` for init-time flags (for example structured logs or metrics). |
| `beforeSend` | Optional `Func<SentryEvent, SentryHint, SentryEvent?>` chained after internal `EventId` handling. |

### Further reading (Sentry .NET **6.3.2**)

- [Data Management](https://docs.sentry.io/platforms/dotnet/data-management/) — overview of collection, scrubbing, and debug symbols.
- [Metrics](https://docs.sentry.io/platforms/dotnet/) (product docs: **Metrics**, beta) — `SentrySdk.Metrics` (SDK **≥ 6.1**); options such as **`EnableMetrics`** and **`SetBeforeSendMetric`** are set on `SentryOptions` at init.
- [Structured logs](https://docs.sentry.io/platforms/dotnet/) (product docs: **Logs**) — **`EnableLogs`**, **`SetBeforeSendLog`**, `SentrySdk.Logger` (SDK **≥ 5.14**).
- [Event processors](https://docs.sentry.io/platforms/dotnet/enriching-events/event-processors/) — differ from **`BeforeSend`** (runs last).
- [Debug information files](https://docs.sentry.io/platforms/dotnet/data-management/debug-files/) — better stack traces for release builds.
- [Migration guide](https://docs.sentry.io/platforms/dotnet/migration/) — upgrading between SDK versions.
