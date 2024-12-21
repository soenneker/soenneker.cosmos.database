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
    public static IServiceCollection AddCosmosDatabaseUtilAsSingleton(this IServiceCollection services)
    {
        services.AddCosmosDatabaseSetupUtilAsSingleton();
        services.TryAddSingleton<ICosmosDatabaseUtil, CosmosDatabaseUtil>();

        return services;
    }
}