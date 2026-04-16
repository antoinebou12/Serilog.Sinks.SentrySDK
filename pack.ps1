#Requires -Version 5.1
<#
.SYNOPSIS
    Builds Release NuGet packages for Serilog.Sinks.SentrySDK (same layout as CI).

.DESCRIPTION
    Restores the solution, then packs the main and AspNetCore projects into ./nupkgs.
    Each package includes lib/net6.0 and lib/net10.0. Use a recent SDK (10.x recommended).

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

Write-Host 'dotnet pack (Release -> nupkgs)' -ForegroundColor Cyan
dotnet pack $core --configuration Release --output $out --nologo
dotnet pack $aspnet --configuration Release --output $out --nologo

Write-Host 'Packages:' -ForegroundColor Green
Get-ChildItem -LiteralPath $out -Filter '*.nupkg' | ForEach-Object { Write-Host "  $($_.FullName)" }
