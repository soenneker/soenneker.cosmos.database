using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Cosmos.Client.Abstract;
using Soenneker.Cosmos.Database.Abstract;
using Soenneker.Cosmos.Database.Setup.Abstract;
using Soenneker.Cosmos.Database.Utils;
using Soenneker.Dictionaries.SingletonKeys;
using Soenneker.Extensions.Configuration;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Cosmos.Database;

/// <inheritdoc cref="ICosmosDatabaseUtil"/>
public sealed class CosmosDatabaseUtil : ICosmosDatabaseUtil
{
    private readonly ILogger<CosmosDatabaseUtil> _logger;

    private readonly SingletonKeyDictionary<CosmosDatabaseKey, Microsoft.Azure.Cosmos.Database, CosmosDatabaseArgs> _databases;

    private readonly ICosmosClientUtil _cosmosClientUtil;
    private readonly ICosmosDatabaseSetupUtil _cosmosDatabaseSetupUtil;
    private readonly bool _ensureDatabaseOnFirstUse;

    private readonly DefaultCosmosConfig _default;

    public CosmosDatabaseUtil(ICosmosClientUtil cosmosClientUtil, ICosmosDatabaseSetupUtil cosmosDatabaseSetupUtil, IConfiguration config,
        ILogger<CosmosDatabaseUtil> logger)
    {
        _logger = logger;
        _cosmosClientUtil = cosmosClientUtil;
        _cosmosDatabaseSetupUtil = cosmosDatabaseSetupUtil;

        _ensureDatabaseOnFirstUse = config.GetValue("Azure:Cosmos:EnsureDatabaseOnFirstUse", true);

        _default = new DefaultCosmosConfig(Endpoint: config.GetValueStrict<string>("Azure:Cosmos:Endpoint"),
            AccountKey: config.GetValueStrict<string>("Azure:Cosmos:AccountKey"), DatabaseName: config.GetValueStrict<string>("Azure:Cosmos:DatabaseName"));

        _databases = new SingletonKeyDictionary<CosmosDatabaseKey, Microsoft.Azure.Cosmos.Database, CosmosDatabaseArgs>(CreateDatabase);
    }

    private async ValueTask<Microsoft.Azure.Cosmos.Database> CreateDatabase(CosmosDatabaseKey _, CosmosDatabaseArgs args, CancellationToken token)
    {
        CosmosClient client = await _cosmosClientUtil.Get(args.Endpoint, args.AccountKey, token)
                                                     .NoSync();

        try
        {
            if (_ensureDatabaseOnFirstUse)
            {
                await _cosmosDatabaseSetupUtil.Ensure(args.Endpoint, args.AccountKey, args.DatabaseName, token)
                                              .NoSync();
            }

            return client.GetDatabase(args.DatabaseName);
        }
        catch (Exception e)
        {
            string message = $"*** CosmosDatabaseUtil *** Failed to get database for endpoint {args.Endpoint ?? "unknown"}. " +
                             "This probably means we were unable to connect to Cosmos. We'll try to connect again next request.";

            _logger.LogCritical(e, "{message}", message);
            throw new Exception(message);
        }
    }

    public ValueTask<Microsoft.Azure.Cosmos.Database> Get(CancellationToken cancellationToken = default) =>
        Get(_default.Endpoint, _default.AccountKey, _default.DatabaseName, cancellationToken);

    public ValueTask<Microsoft.Azure.Cosmos.Database> Get(string endpoint, string accountKey, string databaseName,
        CancellationToken cancellationToken = default)
    {
        var key = new CosmosDatabaseKey(endpoint, databaseName);
        var args = new CosmosDatabaseArgs(endpoint, accountKey, databaseName);

        return _databases.Get(key, args, cancellationToken);
    }

    public ValueTask Delete(CancellationToken cancellationToken = default) =>
        Delete(_default.Endpoint, _default.AccountKey, _default.DatabaseName, cancellationToken);

    public async ValueTask Delete(string endpoint, string accountKey, string databaseName, CancellationToken cancellationToken = default)
    {
        _logger.LogCritical("Deleting database {database} from endpoint {endpoint}! ...", databaseName, endpoint);

        Microsoft.Azure.Cosmos.Database database = await Get(endpoint, accountKey, databaseName, cancellationToken)
            .NoSync();

        await database.DeleteAsync(cancellationToken: cancellationToken)
                      .NoSync();

        var key = new CosmosDatabaseKey(endpoint, databaseName);

        await _databases.Remove(key, cancellationToken)
                        .NoSync();

        _logger.LogWarning("Finished deleting database {database} from endpoint {endpoint}", databaseName, endpoint);
    }

    public ValueTask DisposeAsync() => _databases.DisposeAsync();

    public void Dispose() => _databases.Dispose();
}