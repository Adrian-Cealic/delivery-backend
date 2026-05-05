using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Memento;

/// <summary>
/// Immutable snapshot of an OrderDraft. Internal state is hidden from external callers;
/// only the originating OrderDraft can read it back via the internal API.
/// </summary>
public sealed class OrderDraftMemento
{
    internal IReadOnlyList<OrderDraftLine> Lines { get; }
    internal OrderPriority Priority { get; }
    internal string? DeliveryNotes { get; }
    public string Label { get; }
    public DateTime SavedAt { get; }

    internal OrderDraftMemento(
        string label,
        IEnumerable<OrderDraftLine> lines,
        OrderPriority priority,
        string? deliveryNotes)
    {
        Label = label;
        Lines = lines.Select(l => new OrderDraftLine(l.ProductName, l.Quantity, l.UnitPrice, l.Weight)).ToList();
        Priority = priority;
        DeliveryNotes = deliveryNotes;
        SavedAt = DateTime.UtcNow;
    }
}

public sealed record OrderDraftLine(string ProductName, int Quantity, decimal UnitPrice, decimal Weight);
