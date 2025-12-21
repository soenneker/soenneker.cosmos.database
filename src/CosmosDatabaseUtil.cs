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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Cosmos.Database;

public readonly record struct CosmosDatabaseArgs(string Endpoint, string AccountKey, string DatabaseName);

/// <inheritdoc cref="ICosmosDatabaseUtil"/>
public sealed class CosmosDatabaseUtil : ICosmosDatabaseUtil
{
    private readonly ILogger<CosmosDatabaseUtil> _logger;
    private readonly SingletonDictionary<Microsoft.Azure.Cosmos.Database, CosmosDatabaseArgs> _databases;
    private readonly IConfiguration _config;

    public CosmosDatabaseUtil(ICosmosClientUtil cosmosClientUtil, ICosmosDatabaseSetupUtil cosmosDatabaseSetupUtil, IConfiguration config,
        ILogger<CosmosDatabaseUtil> logger)
    {
        _logger = logger;
        _config = config;

        bool ensureDatabaseOnFirstUse = config.GetValue("Azure:Cosmos:EnsureDatabaseOnFirstUse", true);

        _databases = new SingletonDictionary<Microsoft.Azure.Cosmos.Database, CosmosDatabaseArgs>(async (key, token, args) =>
        {
            CosmosClient client = await cosmosClientUtil.Get(args.Endpoint, args.AccountKey, token)
                                                        .NoSync();

            try
            {
                if (ensureDatabaseOnFirstUse)
                {
                    _ = await cosmosDatabaseSetupUtil.Ensure(args.Endpoint, args.AccountKey, args.DatabaseName, token)
                                                     .NoSync();
                }

                return client.GetDatabase(args.DatabaseName);
            }
            catch (Exception e)
            {
                var message =
                    $"*** CosmosDatabaseUtil *** Failed to get database for endpoint {args.Endpoint ?? "unknown"}. This probably means we were unable to connect to Cosmos. We'll try to connect again next request.";

                logger.LogCritical(e, "{message}", message);

                throw new Exception(message);
            }
        });
    }

    public ValueTask<Microsoft.Azure.Cosmos.Database> Get(CancellationToken cancellationToken = default)
    {
        var databaseName = _config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");
        var endpoint = _config.GetValueStrict<string>("Azure:Cosmos:Endpoint");
        var accountKey = _config.GetValueStrict<string>("Azure:Cosmos:AccountKey");

        return Get(endpoint, accountKey, databaseName, cancellationToken);
    }

    public ValueTask<Microsoft.Azure.Cosmos.Database> Get(string endpoint, string accountKey, string databaseName,
        CancellationToken cancellationToken = default)
    {
        var key = $"{endpoint}-{databaseName}";
        var args = new CosmosDatabaseArgs(endpoint, accountKey, databaseName);

        return _databases.Get(key, args, cancellationToken);
    }

    public ValueTask Delete(CancellationToken cancellationToken = default)
    {
        var databaseName = _config.GetValueStrict<string>("Azure:Cosmos:DatabaseName");
        var endpoint = _config.GetValueStrict<string>("Azure:Cosmos:Endpoint");
        var accountKey = _config.GetValueStrict<string>("Azure:Cosmos:AccountKey");

        return Delete(endpoint, accountKey, databaseName, cancellationToken);
    }

    public async ValueTask Delete(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default)
    {
        _logger.LogCritical("Deleting database {database} from endpoint {endpoint}! ...", databaseName, endpoint);

        Microsoft.Azure.Cosmos.Database database = await Get(endpoint, accountKey, databaseName, cancellationToken)
            .NoSync();

        await database.DeleteAsync(cancellationToken: cancellationToken)
                      .NoSync();

        var key = $"{endpoint}-{databaseName}";
        await _databases.Remove(key, cancellationToken)
                        .NoSync();

        _logger.LogWarning("Finished deleting database {database} from endpoint {endpoint}", databaseName, endpoint);
    }

    public ValueTask DisposeAsync() => _databases.DisposeAsync();

    public void Dispose() => _databases.Dispose();
}