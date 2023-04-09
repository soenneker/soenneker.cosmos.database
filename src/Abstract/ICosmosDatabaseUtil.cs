using System.Diagnostics.Contracts;
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
    ValueTask<Microsoft.Azure.Cosmos.Database> GetDatabase();

    /// <summary>
    /// Implements double check locking mechanism
    /// </summary>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> GetDatabase(string databaseName);

    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> GetDatabase(string databaseName, CosmosClient cosmosClient);

    ValueTask DeleteDatabase();

    ValueTask DeleteDatabase(string databaseName);
}
