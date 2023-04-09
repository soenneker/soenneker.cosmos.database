using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cosmos.Database.Abstract;
using Soenneker.Cosmos.Database.Setup.Registrars;

namespace Soenneker.Cosmos.Database.Registrars;

/// <summary>
/// A utility library for storing Azure Cosmos databases
/// </summary>
public static class CosmosDatabaseUtilRegistrar
{
    /// <summary>
    /// As Singleton
    /// </summary>
    public static void AddCosmosDatabaseUtil(this IServiceCollection services)
    {
        services.AddCosmosDatabaseSetupUtil();
        services.TryAddSingleton<ICosmosDatabaseUtil, CosmosDatabaseUtil>();
    }
}