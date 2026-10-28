using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.Contracts;
using UnitConversion.Application.Conversions;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api/v1/conversions")]
public sealed class ConversionsController(IUnitConversionService conversionService) : ControllerBase
{
    /// <summary>Converts a value from one unit to another using query-string parameters.</summary>
    [HttpGet]
    [ProducesResponseType<ConversionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<ConversionResponse> Convert([FromQuery] ConvertUnitsRequest request) =>
        Execute(request);

    /// <summary>Converts a value from one unit to another using a JSON body.</summary>
    [HttpPost]
    [ProducesResponseType<ConversionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<ConversionResponse> ConvertFromBody([FromBody] ConvertUnitsRequest request) =>
        Execute(request);

    private ActionResult<ConversionResponse> Execute(ConvertUnitsRequest request)
    {
        // [Required] validation guarantees these values are present once the model state is valid.
        var result = conversionService.Convert(request.Value!.Value, request.From!, request.To!);
        return Ok(ConversionResponse.FromResult(result));
    }
}
