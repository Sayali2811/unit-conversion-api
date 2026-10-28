using Microsoft.Extensions.DependencyInjection;
using UnitConversion.Application.Units;
using UnitConversion.Infrastructure.Units;

namespace UnitConversion.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUnitRepository>(_ => new InMemoryUnitRepository(UnitCatalog.GetDefaultUnits()));
        return services;
    }
}
