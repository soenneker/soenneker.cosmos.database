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
    /// Adds cosmos database util as singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The result of the operation.</returns>
    public static IServiceCollection AddCosmosDatabaseUtilAsSingleton(this IServiceCollection services)
    {
        services.AddCosmosDatabaseSetupUtilAsSingleton().TryAddSingleton<ICosmosDatabaseUtil, CosmosDatabaseUtil>();

        return services;
    }
}