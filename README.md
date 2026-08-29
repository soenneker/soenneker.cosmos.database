[![](https://img.shields.io/nuget/v/Soenneker.Cosmos.Database.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Cosmos.Database/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cosmos.database/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cosmos.database/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Cosmos.Database.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Cosmos.Database/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cosmos.database/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cosmos.database/actions/workflows/codeql.yml)

# Soenneker.Cosmos.Database

A utility library for storing Azure Cosmos databases Singleton IoC.

## Install

```bash
dotnet add package Soenneker.Cosmos.Database
```

## Quick start

```csharp
using Soenneker.Cosmos.Database.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddCosmosDatabaseUtilAsSingleton();
```

Registers Cosmos Database Util with a singleton lifetime.

## What you get

- `ICosmosDatabaseUtil` — A utility library for storing Azure Cosmos databases Singleton IoC.
- `CosmosDatabaseUtilRegistrar` — A utility library for storing Azure Cosmos databases.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `ICosmosDatabaseUtil.Get(endpoint, accountKey, databaseName, cancellationToken)` | Implements double check locking mechanism. | A task whose result is the requested microsoft.Azure.Cosmos.Database. |
| `ICosmosDatabaseUtil.Delete(endpoint, accountKey, databaseName, cancellationToken)` | Removes the entry associated with the specified key. | Completes when the requested deletion has finished. |
| `CosmosDatabaseUtilRegistrar.AddCosmosDatabaseUtilAsSingleton(services)` | Registers Cosmos Database Util with a singleton lifetime. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
- Calls that return a cached or singleton value reuse the same instance until the owning service is disposed.
- Dispose instances you own when their scope ends so held resources can be released.
