using UnitConversion.Domain.Units;

namespace UnitConversion.Application.Conversions;

public interface IUnitConversionService
{
    /// <exception cref="Domain.Exceptions.UnknownUnitException">A unit is not supported.</exception>
    /// <exception cref="Domain.Exceptions.IncompatibleUnitsException">The units belong to different categories.</exception>
    /// <exception cref="Domain.Exceptions.ValueOutOfRangeException">The value cannot be represented or is physically impossible.</exception>
    ConversionResult Convert(decimal value, string fromUnit, string toUnit);

    IReadOnlyCollection<UnitDefinition> GetUnits(UnitCategory? category = null);
}
