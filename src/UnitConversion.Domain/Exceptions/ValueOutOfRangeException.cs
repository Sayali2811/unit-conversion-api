namespace UnitConversion.Domain.Exceptions;

public sealed class ValueOutOfRangeException : UnitConversionException
{
    public ValueOutOfRangeException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
