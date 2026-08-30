using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Cosmos.Database.Abstract;

/// <summary>
/// Resolves and caches Azure Cosmos DB database handles.
/// </summary>
public interface ICosmosDatabaseUtil : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Returns the database configured under <c>Azure:Cosmos</c>.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested microsoft.Azure.Cosmos.Database.</returns>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a cached database handle for the specified endpoint, credential, and database name.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested microsoft.Azure.Cosmos.Database.</returns>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> Get(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the database configured under <c>Azure:Cosmos</c> and evicts its cached handle.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after the database has been deleted.</returns>
    ValueTask Delete(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified database and evicts its cached handle.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after the database has been deleted.</returns>
    ValueTask Delete(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);
}
