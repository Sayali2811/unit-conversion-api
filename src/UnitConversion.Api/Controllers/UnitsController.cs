using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.Contracts;
using UnitConversion.Application.Conversions;
using UnitConversion.Domain.Units;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api/v1/units")]
public sealed class UnitsController(IUnitConversionService conversionService) : ControllerBase
{
    /// <summary>Lists supported units, optionally filtered by category (length, mass, temperature).</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<UnitResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<IReadOnlyCollection<UnitResponse>> GetUnits([FromQuery] UnitCategory? category)
    {
        var units = conversionService
            .GetUnits(category)
            .Select(UnitResponse.FromDefinition)
            .ToArray();

        return Ok(units);
    }
}
