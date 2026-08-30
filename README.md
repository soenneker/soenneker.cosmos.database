[![](https://img.shields.io/nuget/v/Soenneker.Cosmos.Database.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Cosmos.Database/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cosmos.database/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.cosmos.database/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Cosmos.Database.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Cosmos.Database/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.cosmos.database/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.cosmos.database/actions/workflows/codeql.yml)

# Soenneker.Cosmos.Database

Resolves and caches Azure Cosmos DB `Database` handles, with optional database creation on first use.

## Installation

```bash
dotnet add package Soenneker.Cosmos.Database
```

## Configuration

```json
{
  "Azure": {
    "Cosmos": {
      "Endpoint": "https://your-account.documents.azure.com:443/",
      "AccountKey": "your-account-key",
      "DatabaseName": "app",
      "EnsureDatabaseOnFirstUse": true
    }
  }
}
```

`Endpoint`, `AccountKey`, and `DatabaseName` are required when the service is constructed. `EnsureDatabaseOnFirstUse` defaults to `true`; set it to `false` when this process must not create databases.

## Registration and use

```csharp
using Soenneker.Cosmos.Database.Abstract;
using Soenneker.Cosmos.Database.Registrars;

services.AddCosmosDatabaseUtilAsSingleton();

ICosmosDatabaseUtil databases = serviceProvider.GetRequiredService<ICosmosDatabaseUtil>();
Microsoft.Azure.Cosmos.Database database = await databases.Get(cancellationToken);
```

The registrar also adds the Cosmos database-setup and client dependencies. The utility is intentionally singleton-scoped so database handles can be reused.

To resolve a database other than the configured default:

```csharp
Microsoft.Azure.Cosmos.Database archive = await databases.Get(
    endpoint,
    accountKey,
    "archive",
    cancellationToken);
```

Handles are cached by endpoint, account-key identity, and database name. Concurrent requests for the same key share one initialization.

## Deleting a database

```csharp
await databases.Delete(endpoint, accountKey, "temporary", cancellationToken);
```

`Delete()` permanently deletes the database in Cosmos DB and removes its cached handle. With `EnsureDatabaseOnFirstUse` enabled, deleting a database that does not exist may create it immediately before deleting it because deletion first resolves the handle.

Cosmos SDK and setup failures propagate to the caller. Cancellation is honored during initialization and deletion. Disposing the registered service releases its cache; the Cosmos client dependency owns the underlying clients.
