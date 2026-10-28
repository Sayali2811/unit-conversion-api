using UnitConversion.Domain.Units;
using UnitConversion.Infrastructure.Units;

namespace UnitConversion.Tests.Infrastructure;

public class InMemoryUnitRepositoryTests
{
    private readonly InMemoryUnitRepository _sut = new(UnitCatalog.GetDefaultUnits());

    [Theory]
    [InlineData("m")]
    [InlineData("M")]
    [InlineData("Metre")]
    [InlineData("meters")]
    public void FindByIdentifier_IsCaseInsensitive_AndSupportsNamesAndAliases(string identifier)
    {
        var unit = _sut.FindByIdentifier(identifier);

        Assert.NotNull(unit);
        Assert.Equal("m", unit.Symbol);
    }

    [Fact]
    public void FindByIdentifier_ReturnsNull_ForUnknownUnit()
    {
        Assert.Null(_sut.FindByIdentifier("parsec"));
    }

    [Fact]
    public void Constructor_Throws_WhenIdentifiersCollide()
    {
        UnitDefinition[] units =
        [
            new("m", "Metre", UnitCategory.Length, 1m),
            new("M", "Duplicate", UnitCategory.Length, 1m)
        ];

        Assert.Throws<InvalidOperationException>(() => new InMemoryUnitRepository(units));
    }

    [Fact]
    public void DefaultCatalog_CoversRequiredCategories()
    {
        var categories = _sut.GetAll().Select(unit => unit.Category).Distinct().ToArray();

        Assert.Contains(UnitCategory.Length, categories);
        Assert.Contains(UnitCategory.Mass, categories);
        Assert.Contains(UnitCategory.Temperature, categories);
    }
}
