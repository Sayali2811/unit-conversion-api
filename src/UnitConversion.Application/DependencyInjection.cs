using Microsoft.Extensions.DependencyInjection;
using UnitConversion.Application.Conversions;

namespace UnitConversion.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IUnitConversionService, UnitConversionService>();
        return services;
    }
}
