using Microsoft.Extensions.Options;
using UnitConversion.Application.Units;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Units;

namespace UnitConversion.Application.Conversions;

/// <summary>
/// Converts between units by normalising through the base unit of their shared category:
/// <c>source -> base -> target</c>. Supporting N units therefore needs N definitions, not N*N conversion pairs.
/// </summary>
public sealed class UnitConversionService : IUnitConversionService
{
    private readonly IUnitRepository _units;
    private readonly ConversionOptions _options;

    public UnitConversionService(IUnitRepository units, IOptions<ConversionOptions> options)
    {
        ArgumentNullException.ThrowIfNull(units);
        ArgumentNullException.ThrowIfNull(options);

        _units = units;
        _options = options.Value;
    }

    public ConversionResult Convert(decimal value, string fromUnit, string toUnit)
    {
        var source = Resolve(fromUnit);
        var target = Resolve(toUnit);

        if (source.Category != target.Category)
        {
            throw new IncompatibleUnitsException(source, target);
        }

        try
        {
            var baseValue = source.ToBase(value);
            EnsureWithinPhysicalLimits(source.Category, baseValue);

            var converted = target.FromBase(baseValue);
            return new ConversionResult(value, source, target, Finalise(converted));
        }
        catch (OverflowException ex)
        {
            throw new ValueOutOfRangeException("The value is too large to be converted.", ex);
        }
    }

    public IReadOnlyCollection<UnitDefinition> GetUnits(UnitCategory? category = null) =>
        _units.GetAll()
            .Where(unit => category is null || unit.Category == category)
            .OrderBy(unit => unit.Category)
            .ThenBy(unit => unit.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private UnitDefinition Resolve(string identifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        var trimmed = identifier.Trim();
        return _units.FindByIdentifier(trimmed) ?? throw new UnknownUnitException(trimmed);
    }

    private static void EnsureWithinPhysicalLimits(UnitCategory category, decimal baseValue)
    {
        if (category.MinimumBaseValue() is { } minimum && baseValue < minimum)
        {
            throw new ValueOutOfRangeException(
                $"The value is below the lowest possible {category.ToString().ToLowerInvariant()} (absolute zero).");
        }
    }

    private decimal Finalise(decimal value)
    {
        var rounded = decimal.Round(value, _options.DecimalPlaces, MidpointRounding.AwayFromZero);

        // Dividing by a decimal with many trailing zeros strips redundant trailing zeros (212.0000 -> 212).
        return rounded / 1.0000000000000000000000000000m;
    }
}
