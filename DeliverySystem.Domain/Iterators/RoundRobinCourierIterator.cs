using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Domain.Iterators;

/// <summary>
/// Cycles through couriers in a wrap-around order for `totalSteps` iterations.
/// Useful for fair workload distribution when assigning consecutive deliveries.
/// </summary>
internal sealed class RoundRobinCourierIterator : IDeliveryIterator<Courier>
{
    private readonly IReadOnlyList<Courier> _items;
    private readonly int _totalSteps;
    private int _stepsTaken;
    private int _cursor;

    public RoundRobinCourierIterator(IReadOnlyList<Courier> items, int totalSteps)
    {
        if (totalSteps < 0)
            throw new ArgumentException("Total steps cannot be negative.", nameof(totalSteps));

        _items = items ?? throw new ArgumentNullException(nameof(items));
        _totalSteps = totalSteps;
    }

    public bool HasNext() => _items.Count > 0 && _stepsTaken < _totalSteps;

    public Courier Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("Round-robin iterator exhausted.");

        var courier = _items[_cursor];
        _cursor = (_cursor + 1) % _items.Count;
        _stepsTaken++;
        return courier;
    }

    public void Reset()
    {
        _stepsTaken = 0;
        _cursor = 0;
    }
}
