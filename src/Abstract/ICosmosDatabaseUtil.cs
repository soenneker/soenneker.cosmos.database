using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Soenneker.Cosmos.Database.Abstract;

/// <summary>
/// A utility library for storing Azure Cosmos databases <para/>
/// Singleton IoC
/// </summary>
public interface ICosmosDatabaseUtil
{
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Implements double check locking mechanism
    /// </summary>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> Get(string databaseName, CancellationToken cancellationToken = default);

    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> Get(string databaseName, CosmosClient cosmosClient, CancellationToken cancellationToken = default);

    ValueTask Delete(CancellationToken cancellationToken = default);

    ValueTask Delete(string databaseName, CancellationToken cancellationToken = default);
}
