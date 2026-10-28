namespace UnitConversion.Domain.Exceptions;

public sealed class UnknownUnitException : UnitConversionException
{
    public UnknownUnitException(string unit)
        : base($"The unit '{unit}' is not supported.")
    {
        Unit = unit;
    }

    public string Unit { get; }
}
