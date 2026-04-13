# Contributing to Serilog.Sinks.SentrySDK

Thank you for helping improve this project. This document describes how we work on GitHub and what we expect in contributions.

## How we use GitHub

- **Code and reviews:** [github.com/antoinebou12/Serilog.Sinks.SentrySDK](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK)
- **Bugs and ideas:** use [Issues](https://github.com/antoinebou12/Serilog.Sinks.SentrySDK/issues). Templates are available for [bug reports](.github/ISSUE_TEMPLATE/bug_report.md) and [feature requests](.github/ISSUE_TEMPLATE/feature_request.md).
- **Security:** do not open public issues for security problems. See [SECURITY.md](SECURITY.md) for how to report them responsibly.
- **Workflow:** we follow a typical [GitHub Flow](https://docs.github.com/en/get-started/using-github/github-flow) model: branch from `main`, open a **pull request**, address review feedback, and merge when CI is green.

## Pull requests

1. Fork the repository and create a branch from `main`.
2. Make focused changes; avoid unrelated refactors in the same PR.
3. **Add or update tests** when you change behavior or fix bugs. We use **xUnit**, **Moq**, and **FluentAssertions** where applicable.
4. **Update documentation** (README, XML comments on public API) when user-facing behavior or configuration changes.
5. Ensure **`dotnet build`** and **`dotnet test`** succeed for the projects you touched (see [README.md](README.md#build-run-tests-and-coverage-local-development)).
6. Open a PR against `main`. Use the [pull request template](.github/PULL_REQUEST_TEMPLATE/pull_request_template.md) when it helps describe the change.

## Tests and coverage

- Run tests locally before pushing:

  ```bash
  dotnet test src/Serilog.Sinks.SentrySDK.Tests/Serilog.Sinks.SentrySDK.Tests.csproj
  dotnet test src/Serilog.Sinks.SentrySDK.AspNetCore.Tests/Serilog.Sinks.SentrySDK.AspNetCore.Tests.csproj
  ```

- CI runs these tests and uploads coverage to Codecov. Prefer **new tests** for new code paths (e.g. factories, sink behavior, middleware) so coverage does not drop unnecessarily.

## Code style

- Follow [.NET/C# coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
- Formatting: the repo uses [`.editorconfig`](.editorconfig). Run `dotnet restore src/Serilog.Sinks.SentrySDK.sln` then `dotnet format src/Serilog.Sinks.SentrySDK.sln` before pushing; CI runs `dotnet format --verify-no-changes` (see [README](README.md#code-formatting-dotnet-format)).
- Keep public API changes backward compatible when possible; document breaking changes in the PR description and release notes.

## License

By contributing, you agree that your contributions are licensed under the same [MIT License](LICENSE) as the project.
