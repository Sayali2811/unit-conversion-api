using UnitConversion.Domain.Units;

namespace UnitConversion.Application.Conversions;

public sealed record ConversionResult(
    decimal InputValue,
    UnitDefinition From,
    UnitDefinition To,
    decimal Result);
