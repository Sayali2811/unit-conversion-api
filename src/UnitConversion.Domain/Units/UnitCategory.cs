namespace UnitConversion.Domain.Units;

/// <summary>
/// A family of units that can be converted into one another.
/// Units of different categories are never convertible.
/// </summary>
public enum UnitCategory
{
    Length,
    Mass,
    Temperature
}
