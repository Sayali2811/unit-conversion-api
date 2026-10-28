using UnitConversion.Domain.Units;

namespace UnitConversion.Domain.Exceptions;

public sealed class IncompatibleUnitsException : UnitConversionException
{
    public IncompatibleUnitsException(UnitDefinition source, UnitDefinition target)
        : base($"Cannot convert {source.Category} unit '{source.Symbol}' to {target.Category} unit '{target.Symbol}'.")
    {
    }
}
