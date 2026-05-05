using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Memento;

/// <summary>
/// Originator: a mutable order under construction whose state can be snapshotted
/// to OrderDraftMemento and later restored. The internal state is encapsulated;
/// the memento only exposes its label/savedAt to the outside world.
/// </summary>
public sealed class OrderDraft
{
    private readonly List<OrderDraftLine> _lines = new();
    private OrderPriority _priority = OrderPriority.Normal;
    private string? _deliveryNotes;

    public IReadOnlyList<OrderDraftLine> Lines => _lines.AsReadOnly();
    public OrderPriority Priority => _priority;
    public string? DeliveryNotes => _deliveryNotes;

    public void AddLine(string productName, int quantity, decimal unitPrice, decimal weight)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required.", nameof(productName));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        _lines.Add(new OrderDraftLine(productName, quantity, unitPrice, weight));
    }

    public void RemoveLineAt(int index)
    {
        if (index < 0 || index >= _lines.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        _lines.RemoveAt(index);
    }

    public void SetPriority(OrderPriority priority) => _priority = priority;
    public void SetDeliveryNotes(string? notes) => _deliveryNotes = notes;

    public decimal Total => _lines.Sum(l => l.Quantity * l.UnitPrice);

    public OrderDraftMemento Save(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Snapshot label is required.", nameof(label));

        return new OrderDraftMemento(label, _lines, _priority, _deliveryNotes);
    }

    public void Restore(OrderDraftMemento memento)
    {
        if (memento == null) throw new ArgumentNullException(nameof(memento));

        _lines.Clear();
        _lines.AddRange(memento.Lines);
        _priority = memento.Priority;
        _deliveryNotes = memento.DeliveryNotes;
    }
}
