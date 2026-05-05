using DeliverySystem.Domain.Memento;

namespace DeliverySystem.Services.Memento;

/// <summary>
/// Caretaker: keeps a stack of order draft snapshots and exposes named restore.
/// Knows nothing about the memento internals — pure storage and lookup.
/// </summary>
public sealed class OrderDraftCaretaker
{
    private readonly List<OrderDraftMemento> _history = new();

    public IReadOnlyList<SnapshotInfo> Snapshots =>
        _history.Select(m => new SnapshotInfo(m.Label, m.SavedAt)).ToList();

    public int Count => _history.Count;

    public void Push(OrderDraftMemento memento)
    {
        if (memento == null) throw new ArgumentNullException(nameof(memento));
        _history.Add(memento);
    }

    public OrderDraftMemento? FindByLabel(string label)
    {
        return _history.LastOrDefault(m => m.Label.Equals(label, StringComparison.OrdinalIgnoreCase));
    }

    public OrderDraftMemento? PeekLast() => _history.Count == 0 ? null : _history[^1];

    public OrderDraftMemento? PopLast()
    {
        if (_history.Count == 0) return null;
        var last = _history[^1];
        _history.RemoveAt(_history.Count - 1);
        return last;
    }

    public void Clear() => _history.Clear();
}

public sealed record SnapshotInfo(string Label, DateTime SavedAt);
