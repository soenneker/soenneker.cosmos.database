using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Cosmos.Client.Abstract;
using Soenneker.Cosmos.Database.Abstract;
using Soenneker.Cosmos.Database.Setup.Abstract;
using Soenneker.Utils.SingletonDictionary;

namespace Soenneker.Cosmos.Database;

/// <inheritdoc cref="ICosmosDatabaseUtil"/>
public class CosmosDatabaseUtil : ICosmosDatabaseUtil
{
    private readonly ILogger<CosmosDatabaseUtil> _logger;

    private readonly SingletonDictionary<Microsoft.Azure.Cosmos.Database> _databases;

    private bool _ensureDatabaseOnFirstUse;
    private string? _databaseName;

    public CosmosDatabaseUtil(ICosmosClientUtil cosmosClientUtil, ICosmosDatabaseSetupUtil cosmosSetupUtil, IConfiguration config, ILogger<CosmosDatabaseUtil> logger)
    {
        _logger = logger;

        SetConfiguration(config);

        _databases = new SingletonDictionary<Microsoft.Azure.Cosmos.Database>(async args =>
        {
            CosmosClient client = await cosmosClientUtil.GetClient();

            var databaseName = (string) args![0];

            if (_ensureDatabaseOnFirstUse)
                _ = await cosmosSetupUtil.EnsureDatabase(databaseName);

            Microsoft.Azure.Cosmos.Database database = client.GetDatabase(databaseName);

            return database;
        });
    }

    private void SetConfiguration(IConfiguration config)
    {
        _ensureDatabaseOnFirstUse = config.GetValue<bool>("Azure:Cosmos:EnsureDatabaseOnFirstUse");
        _databaseName = config.GetValue<string>("Azure:Cosmos:DatabaseName");

        if (_databaseName == null)
            throw new Exception("Azure:Cosmos:DatabaseName is required");
    }

    public ValueTask<Microsoft.Azure.Cosmos.Database> GetDatabase()
    {
        return _databases.Get(_databaseName!, _databaseName!);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Database> GetDatabase(string databaseName)
    {
        return _databases.Get(databaseName, databaseName);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Database> GetDatabase(string databaseName, CosmosClient cosmosClient)
    {
        int hashOfClient = cosmosClient.GetHashCode();

        var databaseKey = $"{databaseName}-{hashOfClient}";

        return _databases.Get(databaseKey, databaseName);
    }

    public ValueTask DeleteDatabase()
    {
        return DeleteDatabase(_databaseName!);
    }

    public async ValueTask DeleteDatabase(string databaseName)
    {
        _logger.LogCritical("Deleting database {database}! ...", databaseName);

        Microsoft.Azure.Cosmos.Database database = await GetDatabase(databaseName);
        await database.DeleteAsync();

        await _databases.Remove(databaseName);

        _logger.LogWarning("Finished deleting database {database}", databaseName);
    }
}