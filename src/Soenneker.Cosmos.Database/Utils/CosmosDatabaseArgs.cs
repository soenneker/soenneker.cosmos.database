namespace Soenneker.Cosmos.Database.Utils;

/// <summary>
/// Represents the cosmos database args record structure.
/// </summary>
/// <param name="Endpoint">The endpoint.</param>
/// <param name="AccountKey">The account key.</param>
/// <param name="DatabaseName">The database name.</param>
public readonly record struct CosmosDatabaseArgs(string Endpoint, string AccountKey, string DatabaseName);