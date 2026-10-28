using UnitConversion.Domain.Units;

namespace UnitConversion.Infrastructure.Units;

/// <summary>
/// The hard-coded unit data for this version of the system.
/// Base units: metre (length), kilogram (mass), kelvin (temperature).
/// To add a unit, add one line below; no conversion logic needs to change.
/// </summary>
public static class UnitCatalog
{
    public static IReadOnlyList<UnitDefinition> GetDefaultUnits() =>
    [
        .. LengthUnits(),
        .. MassUnits(),
        .. TemperatureUnits()
    ];

    private static UnitDefinition[] LengthUnits() =>
    [
        new("mm", "Millimetre", UnitCategory.Length, 0.001m, aliases: ["millimeter", "millimeters", "millimetres"]),
        new("cm", "Centimetre", UnitCategory.Length, 0.01m, aliases: ["centimeter", "centimeters", "centimetres"]),
        new("m", "Metre", UnitCategory.Length, 1m, aliases: ["meter", "meters", "metres"]),
        new("km", "Kilometre", UnitCategory.Length, 1000m, aliases: ["kilometer", "kilometers", "kilometres"]),
        new("in", "Inch", UnitCategory.Length, 0.0254m, aliases: ["inches"]),
        new("ft", "Foot", UnitCategory.Length, 0.3048m, aliases: ["feet"]),
        new("yd", "Yard", UnitCategory.Length, 0.9144m, aliases: ["yards"]),
        new("mi", "Mile", UnitCategory.Length, 1609.344m, aliases: ["miles"]),
        new("nmi", "Nautical mile", UnitCategory.Length, 1852m, aliases: ["nautical miles"])
    ];

    private static UnitDefinition[] MassUnits() =>
    [
        new("mg", "Milligram", UnitCategory.Mass, 0.000001m, aliases: ["milligrams"]),
        new("g", "Gram", UnitCategory.Mass, 0.001m, aliases: ["grams"]),
        new("kg", "Kilogram", UnitCategory.Mass, 1m, aliases: ["kilograms"]),
        new("t", "Tonne", UnitCategory.Mass, 1000m, aliases: ["tonnes", "metric ton", "metric tons"]),
        new("oz", "Ounce", UnitCategory.Mass, 0.028349523125m, aliases: ["ounces"]),
        new("lb", "Pound", UnitCategory.Mass, 0.45359237m, aliases: ["pounds", "lbs"]),
        new("st", "Stone", UnitCategory.Mass, 6.35029318m, aliases: ["stones"])
    ];

    // Temperature uses: kelvin = (value + offset) * factor
    private static UnitDefinition[] TemperatureUnits() =>
    [
        new("C", "Celsius", UnitCategory.Temperature, 1m, 273.15m, ["°C", "degC"]),
        new("F", "Fahrenheit", UnitCategory.Temperature, 5m / 9m, 459.67m, ["°F", "degF"]),
        new("K", "Kelvin", UnitCategory.Temperature, 1m, 0m),
        new("R", "Rankine", UnitCategory.Temperature, 5m / 9m, 0m, ["°R", "degR"])
    ];
}
