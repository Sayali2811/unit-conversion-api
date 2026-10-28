using UnitConversion.Domain.Units;

namespace UnitConversion.Api.Contracts;

public sealed record UnitResponse(
    string Symbol,
    string Name,
    string Category,
    IReadOnlyCollection<string> Aliases)
{
    public static UnitResponse FromDefinition(UnitDefinition unit) => new(
        unit.Symbol,
        unit.Name,
        unit.Category.ToString(),
        unit.Aliases);
}
