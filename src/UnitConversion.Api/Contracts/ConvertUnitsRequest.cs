using System.ComponentModel.DataAnnotations;

namespace UnitConversion.Api.Contracts;

/// <summary>Input for a conversion; bound from the query string (GET) or the JSON body (POST).</summary>
public sealed class ConvertUnitsRequest
{
    /// <summary>The number to convert.</summary>
    [Required]
    public decimal? Value { get; init; }

    /// <summary>Symbol, name or alias of the source unit, e.g. "m", "Celsius", "pounds".</summary>
    [Required]
    [StringLength(50)]
    public string? From { get; init; }

    /// <summary>Symbol, name or alias of the target unit.</summary>
    [Required]
    [StringLength(50)]
    public string? To { get; init; }
}
