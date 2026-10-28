using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UnitConversion.Domain.Exceptions;

namespace UnitConversion.Api.ErrorHandling;

/// <summary>
/// Translates expected domain failures into RFC 9457 problem responses.
/// Anything else falls through to the default handler and becomes a generic 500.
/// </summary>
public sealed class UnitConversionExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not UnitConversionException conversionException)
        {
            return false;
        }

        var (status, title) = conversionException switch
        {
            UnknownUnitException => (StatusCodes.Status404NotFound, "Unknown unit"),
            IncompatibleUnitsException => (StatusCodes.Status400BadRequest, "Incompatible units"),
            ValueOutOfRangeException => (StatusCodes.Status400BadRequest, "Value out of range"),
            _ => (StatusCodes.Status400BadRequest, "Conversion failed")
        };

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = conversionException.Message
            }
        });
    }
}
