namespace Soenneker.Cosmos.Database.Utils;

public readonly record struct CosmosDatabaseKey(string Endpoint, string DatabaseName)
{
    /// <summary>
    /// SHA-256 identity of the account key used to obtain the database handle. The credential itself is never stored in the cache key.
    /// </summary>
    public string? AccountKeyHash { get; init; }
}
