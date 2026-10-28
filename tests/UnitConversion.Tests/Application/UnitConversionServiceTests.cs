using Microsoft.Extensions.Options;
using UnitConversion.Application.Conversions;
using UnitConversion.Domain.Exceptions;
using UnitConversion.Domain.Units;
using UnitConversion.Infrastructure.Units;

namespace UnitConversion.Tests.Application;

public class UnitConversionServiceTests
{
    private readonly UnitConversionService _sut = new(
        new InMemoryUnitRepository(UnitCatalog.GetDefaultUnits()),
        Options.Create(new ConversionOptions()));

    public static TheoryData<decimal, string, string, decimal> ValidConversions => new()
    {
        // length
        { 1m, "km", "m", 1000m },
        { 100m, "m", "ft", 328.0839895013m },
        { 12m, "in", "ft", 1m },
        { 1m, "mi", "km", 1.609344m },
        { 1m, "nmi", "m", 1852m },
        // mass
        { 500m, "g", "kg", 0.5m },
        { 1m, "kg", "lb", 2.2046226218m },
        { 16m, "oz", "lb", 1m },
        { 1m, "st", "lb", 14m },
        // temperature
        { 0m, "C", "F", 32m },
        { 100m, "C", "F", 212m },
        { 32m, "F", "C", 0m },
        { -40m, "C", "F", -40m },
        { 0m, "K", "C", -273.15m },
        { 0m, "C", "K", 273.15m },
        { -459.67m, "F", "K", 0m },
        // same unit, and name / alias / mixed-case identifiers
        { 42m, "m", "m", 42m },
        { 2m, "KILOMETRES", "metres", 2000m },
        { 1m, "Pounds", "oz", 16m }
    };

    [Theory]
    [MemberData(nameof(ValidConversions))]
    public void Convert_ReturnsExpectedResult(decimal value, string from, string to, decimal expected)
    {
        var result = _sut.Convert(value, from, to);

        Assert.Equal(expected, result.Result, 8);
    }

    [Fact]
    public void Convert_ReturnsResolvedUnits()
    {
        var result = _sut.Convert(1m, "Kilometre", "meters");

        Assert.Equal("km", result.From.Symbol);
        Assert.Equal("m", result.To.Symbol);
        Assert.Equal(1m, result.InputValue);
    }

    [Fact]
    public void Convert_TrimsWhitespaceAroundIdentifiers()
    {
        var result = _sut.Convert(1m, "  km ", " m  ");

        Assert.Equal(1000m, result.Result);
    }

    [Fact]
    public void Convert_RoundsToConfiguredDecimalPlaces()
    {
        var sut = new UnitConversionService(
            new InMemoryUnitRepository(UnitCatalog.GetDefaultUnits()),
            Options.Create(new ConversionOptions { DecimalPlaces = 2 }));

        var result = sut.Convert(1m, "m", "ft");

        Assert.Equal(3.28m, result.Result);
    }

    [Fact]
    public void Convert_Throws_ForUnknownSourceUnit()
    {
        var ex = Assert.Throws<UnknownUnitException>(() => _sut.Convert(1m, "parsec", "m"));

        Assert.Equal("parsec", ex.Unit);
    }

    [Fact]
    public void Convert_Throws_ForUnknownTargetUnit()
    {
        Assert.Throws<UnknownUnitException>(() => _sut.Convert(1m, "m", "parsec"));
    }

    [Theory]
    [InlineData("m", "kg")]
    [InlineData("C", "lb")]
    [InlineData("g", "K")]
    public void Convert_Throws_WhenUnitsBelongToDifferentCategories(string from, string to)
    {
        Assert.Throws<IncompatibleUnitsException>(() => _sut.Convert(1m, from, to));
    }

    [Theory]
    [InlineData(-300, "C", "K")]
    [InlineData(-460, "F", "C")]
    [InlineData(-1, "K", "C")]
    public void Convert_Throws_WhenTemperatureIsBelowAbsoluteZero(int value, string from, string to)
    {
        Assert.Throws<ValueOutOfRangeException>(() => _sut.Convert(value, from, to));
    }

    [Fact]
    public void Convert_Throws_WhenResultOverflows()
    {
        Assert.Throws<ValueOutOfRangeException>(() => _sut.Convert(decimal.MaxValue, "m", "mm"));
    }

    [Fact]
    public void Convert_Throws_ForBlankIdentifier()
    {
        Assert.ThrowsAny<ArgumentException>(() => _sut.Convert(1m, " ", "m"));
    }

    [Fact]
    public void GetUnits_ReturnsEverything_WhenNoCategoryIsGiven()
    {
        var units = _sut.GetUnits();

        Assert.Equal(UnitCatalog.GetDefaultUnits().Count, units.Count);
    }

    [Fact]
    public void GetUnits_FiltersByCategory()
    {
        var units = _sut.GetUnits(UnitCategory.Temperature);

        Assert.NotEmpty(units);
        Assert.All(units, unit => Assert.Equal(UnitCategory.Temperature, unit.Category));
    }
}
