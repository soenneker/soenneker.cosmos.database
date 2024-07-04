using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Cosmos.Client.Abstract;
using Soenneker.Cosmos.Database.Abstract;
using Soenneker.Cosmos.Database.Setup.Abstract;
using Soenneker.Extensions.Configuration;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Utils.SingletonDictionary;

namespace Soenneker.Cosmos.Database;

/// <inheritdoc cref="ICosmosDatabaseUtil"/>
public class CosmosDatabaseUtil : ICosmosDatabaseUtil
{
    private readonly ILogger<CosmosDatabaseUtil> _logger;

    private readonly SingletonDictionary<Microsoft.Azure.Cosmos.Database> _databases;

    private bool _ensureDatabaseOnFirstUse;
    private string? _databaseName;
    private string? _endpoint;

    public CosmosDatabaseUtil(ICosmosClientUtil cosmosClientUtil, ICosmosDatabaseSetupUtil cosmosDatabaseSetupUtil, IConfiguration config, ILogger<CosmosDatabaseUtil> logger)
    {
        _logger = logger;

        SetConfiguration(config);

        _databases = new SingletonDictionary<Microsoft.Azure.Cosmos.Database>(async (key, token, objects) =>
        {
            CosmosClient client = await cosmosClientUtil.Get(token).NoSync();

            var databaseName = (string)objects[0];

            Microsoft.Azure.Cosmos.Database database;

            try
            {
                if (_ensureDatabaseOnFirstUse)
                    _ = await cosmosDatabaseSetupUtil.Ensure(databaseName, token).NoSync();

                database = client.GetDatabase(databaseName);
            }
            catch (Exception e)
            {
                var message =
                    $"*** CosmosDatabaseUtil *** Failed to get database for endpoint {_endpoint}. This probably means we were unable to connect to Cosmos. We'll try to connect again next request.";

                _logger.LogCritical(e, "{message}", message);

                throw new Exception(message);
            }

            return database;
        });
    }

    private void SetConfiguration(IConfiguration config)
    {
        _databaseName = config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");
        _ensureDatabaseOnFirstUse = config.GetValueStrict<bool>("Azure:Cosmos:EnsureDatabaseOnFirstUse");
        _endpoint = config.GetValueStrict<string>("Azure:Cosmos:Endpoint");
    }

    public ValueTask<Microsoft.Azure.Cosmos.Database> Get(CancellationToken cancellationToken = default)
    {
        return _databases.Get(_databaseName!, cancellationToken, _databaseName!);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Database> Get(string databaseName, CancellationToken cancellationToken = default)
    {
        return _databases.Get(databaseName, cancellationToken, databaseName);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Database> Get(string databaseName, CosmosClient cosmosClient, CancellationToken cancellationToken = default)
    {
        int hashOfClient = cosmosClient.GetHashCode();

        var databaseKey = $"{databaseName}-{hashOfClient}";

        return _databases.Get(databaseKey, cancellationToken, databaseName);
    }

    public ValueTask Delete(CancellationToken cancellationToken = default)
    {
        return Delete(_databaseName!, cancellationToken);
    }

    public async ValueTask Delete(string databaseName, CancellationToken cancellationToken = default)
    {
        _logger.LogCritical("Deleting database {database}! ...", databaseName);

        Microsoft.Azure.Cosmos.Database database = await Get(databaseName, cancellationToken).NoSync();
        await database.DeleteAsync(cancellationToken: cancellationToken).NoSync();

        await _databases.Remove(databaseName, cancellationToken).NoSync();

        _logger.LogWarning("Finished deleting database {database}", databaseName);
    }
}