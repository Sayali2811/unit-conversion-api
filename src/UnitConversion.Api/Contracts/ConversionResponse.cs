using UnitConversion.Application.Conversions;

namespace UnitConversion.Api.Contracts;

public sealed record ConversionResponse(
    string Category,
    decimal Value,
    string From,
    string To,
    decimal Result)
{
    public static ConversionResponse FromResult(ConversionResult result) => new(
        result.From.Category.ToString(),
        result.InputValue,
        result.From.Symbol,
        result.To.Symbol,
        result.Result);
}
