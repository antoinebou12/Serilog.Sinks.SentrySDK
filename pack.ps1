#Requires -Version 5.1
<#
.SYNOPSIS
    Builds Release NuGet packages for Serilog.Sinks.SentrySDK (same layout as CI).

.DESCRIPTION
    Restores the solution, then packs four NuGet packages into ./nupkgs:
    Serilog.Sinks.SentrySDK and Serilog.Sinks.SentrySDK.AspNetCore (lib/net6.0 + lib/net10.0),
    plus Serilog.Sinks.SentrySDK.6 and Serilog.Sinks.SentrySDK.AspNetCore.6 (net6.0-only).
    Use a recent SDK (10.x recommended).

.EXAMPLE
    ./pack.ps1
#>
$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot

$sln = Join-Path $PSScriptRoot 'src/Serilog.Sinks.SentrySDK.sln'
$core = Join-Path $PSScriptRoot 'src/Serilog.Sinks.SentrySDK/Serilog.Sinks.SentrySDK.csproj'
$aspnet = Join-Path $PSScriptRoot 'src/Serilog.Sinks.SentrySDK.AspNetCore/Serilog.Sinks.SentrySDK.AspNetCore.csproj'
$out = Join-Path $PSScriptRoot 'nupkgs'

New-Item -ItemType Directory -Force -Path $out | Out-Null

Write-Host 'dotnet restore' -ForegroundColor Cyan
dotnet restore $sln

Write-Host 'dotnet pack (Release -> nupkgs): main + AspNetCore, then .6 net6-only line' -ForegroundColor Cyan
dotnet pack $core --configuration Release --output $out --nologo
dotnet pack $aspnet --configuration Release --output $out --nologo
dotnet pack $core --configuration Release --output $out --nologo -p:AlternateNuGetSuffix=6
dotnet pack $aspnet --configuration Release --output $out --nologo -p:AlternateNuGetSuffix=6

Write-Host 'Packages:' -ForegroundColor Green
Get-ChildItem -LiteralPath $out -Filter '*.nupkg' | ForEach-Object { Write-Host "  $($_.FullName)" }
