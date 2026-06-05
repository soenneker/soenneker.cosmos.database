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
    /// Gets the value.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Implements double check locking mechanism
    /// </summary>
    [Pure]
    ValueTask<Microsoft.Azure.Cosmos.Database> Get(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the delete operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Delete(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the delete operation.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="accountKey">The account key.</param>
    /// <param name="databaseName">The database name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Delete(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default);
}