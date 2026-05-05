using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Iterators;

internal sealed class AvailableCourierIterator : IDeliveryIterator<Courier>
{
    private readonly IReadOnlyList<Courier> _items;
    private int _cursor;

    public AvailableCourierIterator(IReadOnlyList<Courier> items)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
    }

    public bool HasNext()
    {
        for (int i = _cursor; i < _items.Count; i++)
        {
            if (_items[i].IsAvailable) return true;
        }
        return false;
    }

    public Courier Next()
    {
        while (_cursor < _items.Count)
        {
            var candidate = _items[_cursor++];
            if (candidate.IsAvailable) return candidate;
        }
        throw new InvalidOperationException("No more available couriers.");
    }

    public void Reset() => _cursor = 0;
}
