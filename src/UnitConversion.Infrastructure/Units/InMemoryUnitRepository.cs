using UnitConversion.Application.Units;
using UnitConversion.Domain.Units;

namespace UnitConversion.Infrastructure.Units;

public sealed class InMemoryUnitRepository : IUnitRepository
{
    private readonly UnitDefinition[] _units;
    private readonly Dictionary<string, UnitDefinition> _index = new(StringComparer.OrdinalIgnoreCase);

    public InMemoryUnitRepository(IEnumerable<UnitDefinition> units)
    {
        ArgumentNullException.ThrowIfNull(units);

        _units = units.ToArray();

        foreach (var unit in _units)
        {
            foreach (var identifier in unit.Identifiers())
            {
                if (!_index.TryAdd(identifier, unit))
                {
                    throw new InvalidOperationException(
                        $"The unit identifier '{identifier}' is defined more than once.");
                }
            }
        }
    }

    public IReadOnlyCollection<UnitDefinition> GetAll() => _units;

    public UnitDefinition? FindByIdentifier(string identifier) =>
        _index.GetValueOrDefault(identifier);
}
