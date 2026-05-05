using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Iterators;

internal sealed class VehicleTypeCourierIterator : IDeliveryIterator<Courier>
{
    private readonly IReadOnlyList<Courier> _items;
    private readonly VehicleType _vehicleType;
    private int _cursor;

    public VehicleTypeCourierIterator(IReadOnlyList<Courier> items, VehicleType vehicleType)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _vehicleType = vehicleType;
    }

    public bool HasNext()
    {
        for (int i = _cursor; i < _items.Count; i++)
        {
            if (_items[i].VehicleType == _vehicleType) return true;
        }
        return false;
    }

    public Courier Next()
    {
        while (_cursor < _items.Count)
        {
            var candidate = _items[_cursor++];
            if (candidate.VehicleType == _vehicleType) return candidate;
        }
        throw new InvalidOperationException($"No more couriers of type {_vehicleType}.");
    }

    public void Reset() => _cursor = 0;
}
