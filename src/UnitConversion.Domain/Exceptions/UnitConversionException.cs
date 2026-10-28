namespace UnitConversion.Domain.Exceptions;

/// <summary>Base type for all expected, business-level conversion failures.</summary>
public abstract class UnitConversionException : Exception
{
    protected UnitConversionException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
