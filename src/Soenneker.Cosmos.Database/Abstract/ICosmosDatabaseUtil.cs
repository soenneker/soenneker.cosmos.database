using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Cosmos.Database.Abstract;

/// <summary>
/// A utility library for storing Azure Cosmos databases <para/>
/// Singleton IoC
/// </summary>
public interface ICosmosDatabaseUtil : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Returns the configured microsoft.Azure.Cosmos.Database used by the cosmos database.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested microsoft.Azure.Cosmos.Database.</returns>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Implements double check locking mechanism
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested microsoft.Azure.Cosmos.Database.</returns>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> Get(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes cosmos database for the cosmos database.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after the targeted files have been deleted.</returns>
    ValueTask Delete(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the entry associated with the specified key.
    /// </summary>
    /// <param name="endpoint">Service endpoint to call.</param>
    /// <param name="accountKey">Account key used for authentication.</param>
    /// <param name="databaseName">Name of the target database.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes after the targeted files have been deleted.</returns>
    ValueTask Delete(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);
}
