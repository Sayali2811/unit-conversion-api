namespace UnitConversion.Domain.Units;

public static class UnitCategoryExtensions
{
    /// <summary>
    /// The lowest physically meaningful value of a category, expressed in the category's base unit,
    /// or <c>null</c> when the category has no lower bound.
    /// </summary>
    public static decimal? MinimumBaseValue(this UnitCategory category) => category switch
    {
        UnitCategory.Temperature => 0m, // absolute zero, in kelvin
        _ => null
    };
}
