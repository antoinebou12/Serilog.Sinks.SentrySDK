# Serilog.Sinks.SentrySDK

[![NuGet Serilog.Sinks.SentrySDK](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.svg?label=Serilog.Sinks.SentrySDK&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK)
[![NuGet Serilog.Sinks.SentrySDK downloads](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.svg?label=downloads&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK)
[![NuGet Serilog.Sinks.SentrySDK.AspNetCore](https://img.shields.io/nuget/v/Serilog.Sinks.SentrySDK.AspNetCore.svg?label=Serilog.Sinks.SentrySDK.AspNetCore&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore)
[![NuGet Serilog.Sinks.SentrySDK.AspNetCore downloads](https://img.shields.io/nuget/dt/Serilog.Sinks.SentrySDK.AspNetCore.svg&label=downloads&logo=nuget)](https://www.nuget.org/packages/Serilog.Sinks.SentrySDK.AspNetCore)

A Serilog sink for Sentry that simplifies error and log management. It builds on the Sentry .NET SDK and official Serilog integration.

## Release 1.0.7.2 and target frameworks

This line is **1.0.7.2** on NuGet; align GitHub releases with the same version (for example tag **`v1.0.7.2`**). **`Serilog.Sinks.SentrySDK`** and **`Serilog.Sinks.SentrySDK.AspNetCore`** ship **net6.0** and **net10.0** (`lib/net6.0/`, `lib/net10.0/`). **`Serilog.Sinks.SentrySDK.6`** and **`Serilog.Sinks.SentrySDK.AspNetCore.6`** are **net6.0-only** (`lib/net6.0/` only).

## Packages

- **Serilog.Sinks.SentrySDK** — core sink and configuration helpers (net6 + net10).
- **Serilog.Sinks.SentrySDK.AspNetCore** — ASP.NET Core middleware and HTTP context integration (net6 + net10).
- **Serilog.Sinks.SentrySDK.6** / **Serilog.Sinks.SentrySDK.AspNetCore.6** — same product line, alternate NuGet ids, **net6.0-only**.

Install from NuGet:

```bash
dotnet add package Serilog.Sinks.SentrySDK
dotnet add package Serilog.Sinks.SentrySDK.AspNetCore
# net6.0-only package ids:
dotnet add package Serilog.Sinks.SentrySDK.6
dotnet add package Serilog.Sinks.SentrySDK.AspNetCore.6
```

## Documentation

Full readme, demos, and build instructions: [github.com/antoinebou12/Serilog.Sinks.SentrySDK](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/blob/main/README.md)

Releases and changelog: [GitHub Releases](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/releases)

Sentry .NET documentation: [docs.sentry.io/platforms/dotnet](https://docs.sentry.io/platforms/dotnet/)
