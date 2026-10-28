using UnitConversion.Domain.Units;

namespace UnitConversion.Tests.Domain;

public class UnitDefinitionTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_RejectsNonPositiveFactor(int factor)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new UnitDefinition("x", "Test", UnitCategory.Length, factor));
    }

    [Fact]
    public void Constructor_RejectsBlankSymbol()
    {
        Assert.ThrowsAny<ArgumentException>(
            () => new UnitDefinition(" ", "Test", UnitCategory.Length, 1m));
    }

    [Fact]
    public void ToBase_And_FromBase_AreInverse_ForOffsetUnits()
    {
        var fahrenheit = new UnitDefinition("F", "Fahrenheit", UnitCategory.Temperature, 5m / 9m, 459.67m);

        var roundTripped = fahrenheit.FromBase(fahrenheit.ToBase(98.6m));

        Assert.Equal(98.6m, roundTripped, 10);
    }

    [Fact]
    public void Identifiers_AreDistinct_AndIncludeSymbolNameAndAliases()
    {
        var unit = new UnitDefinition("m", "Metre", UnitCategory.Length, 1m, aliases: ["meter", "METRE"]);

        var identifiers = unit.Identifiers().ToArray();

        Assert.Equal(3, identifiers.Length);
        Assert.Contains("m", identifiers);
        Assert.Contains("meter", identifiers);
    }
}
