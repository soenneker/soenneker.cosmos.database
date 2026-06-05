namespace Soenneker.Cosmos.Database.Utils;

/// <summary>
/// Represents the cosmos database key record structure.
/// </summary>
/// <param name="Endpoint">The endpoint.</param>
/// <param name="DatabaseName">The database name.</param>
public readonly record struct CosmosDatabaseKey(string Endpoint, string DatabaseName);