namespace UnitConversion.Domain.Units;

/// <summary>
/// Describes a unit relative to the base unit of its category.
/// <para>
/// The relationship is <c>base = (value + Offset) * FactorToBase</c>. This single affine form covers
/// pure scaling (metres, kilograms) as well as offset scales (Celsius, Fahrenheit).
/// </para>
/// </summary>
public sealed class UnitDefinition
{
    public UnitDefinition(
        string symbol,
        string name,
        UnitCategory category,
        decimal factorToBase,
        decimal offset = 0m,
        IEnumerable<string>? aliases = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (factorToBase <= 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(factorToBase), factorToBase, "The conversion factor must be greater than zero.");
        }

        Symbol = symbol;
        Name = name;
        Category = category;
        FactorToBase = factorToBase;
        Offset = offset;
        Aliases = aliases?.ToArray() ?? [];
    }

    public string Symbol { get; }

    public string Name { get; }

    public UnitCategory Category { get; }

    public decimal FactorToBase { get; }

    public decimal Offset { get; }

    public IReadOnlyCollection<string> Aliases { get; }

    /// <summary>Converts a value in this unit to the category's base unit.</summary>
    public decimal ToBase(decimal value) => (value + Offset) * FactorToBase;

    /// <summary>Converts a value in the category's base unit to this unit.</summary>
    public decimal FromBase(decimal baseValue) => (baseValue / FactorToBase) - Offset;

    /// <summary>All identifiers (symbol, name, aliases) that can be used to look this unit up.</summary>
    public IEnumerable<string> Identifiers() =>
        new[] { Symbol, Name }
            .Concat(Aliases)
            .Distinct(StringComparer.OrdinalIgnoreCase);

    public override string ToString() => $"{Name} ({Symbol})";
}
