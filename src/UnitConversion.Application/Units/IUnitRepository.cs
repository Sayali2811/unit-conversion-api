using UnitConversion.Domain.Units;

namespace UnitConversion.Application.Units;

/// <summary>
/// Read-only access to the catalogue of supported units.
/// Today it is backed by in-memory data; a database or remote store can replace it without touching callers.
/// </summary>
public interface IUnitRepository
{
    IReadOnlyCollection<UnitDefinition> GetAll();

    /// <summary>Finds a unit by symbol, name or alias (case-insensitive). Returns <c>null</c> when unknown.</summary>
    UnitDefinition? FindByIdentifier(string identifier);
}
