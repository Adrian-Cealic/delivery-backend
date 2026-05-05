using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Iterators;

internal sealed class InsertionOrderCourierIterator : IDeliveryIterator<Courier>
{
    private readonly IReadOnlyList<Courier> _items;
    private int _cursor;

    public InsertionOrderCourierIterator(IReadOnlyList<Courier> items)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
    }

    public bool HasNext() => _cursor < _items.Count;

    public Courier Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("Iterator has been exhausted.");
        return _items[_cursor++];
    }

    public void Reset() => _cursor = 0;
}
