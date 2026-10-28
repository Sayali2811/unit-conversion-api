namespace UnitConversion.Application.Conversions;

public sealed class ConversionOptions
{
    public const string SectionName = "Conversion";

    /// <summary>Number of decimal places results are rounded to (0-28).</summary>
    public int DecimalPlaces { get; set; } = 10;
}
