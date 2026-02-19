using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Prototypes;

namespace DeliverySystem.Domain.Entities;

public class Order : EntityBase, IPrototype<Order>
{
    private readonly List<OrderItem> _items;

    public Guid CustomerId { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public OrderStatus Status { get; private set; }
    public OrderPriority Priority { get; private set; }
    public string? DeliveryNotes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Order(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));

        CustomerId = customerId;
        _items = new List<OrderItem>();
        Status = OrderStatus.Created;
        Priority = OrderPriority.Normal;
        CreatedAt = DateTime.UtcNow;
    }

    public Order(Guid id, Guid customerId) : base(id)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));

        CustomerId = customerId;
        _items = new List<OrderItem>();
        Status = OrderStatus.Created;
        Priority = OrderPriority.Normal;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Cannot add items to an order that is not in Created status.");

        _items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Cannot remove items from an order that is not in Created status.");

        _items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPriority(OrderPriority priority)
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Cannot change priority after order has been confirmed.");

        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDeliveryNotes(string? notes)
    {
        DeliveryNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        ValidateStatusTransition(newStatus);
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    private void ValidateStatusTransition(OrderStatus newStatus)
    {
        bool isValidTransition = (Status, newStatus) switch
        {
            (OrderStatus.Created, OrderStatus.Confirmed) => true,
            (OrderStatus.Created, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Processing) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            (OrderStatus.Processing, OrderStatus.ReadyForDelivery) => true,
            (OrderStatus.Processing, OrderStatus.Cancelled) => true,
            (OrderStatus.ReadyForDelivery, OrderStatus.InDelivery) => true,
            (OrderStatus.InDelivery, OrderStatus.Delivered) => true,
            _ => false
        };

        if (!isValidTransition)
            throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}.");
    }

    public decimal GetTotalPrice()
    {
        return _items.Sum(item => item.GetTotalPrice());
    }

    public decimal GetTotalWeight()
    {
        return _items.Sum(item => item.GetTotalWeight());
    }

    public Order Clone()
    {
        var clone = new Order(CustomerId);
        clone.Priority = Priority;
        clone.DeliveryNotes = DeliveryNotes;

        foreach (var item in _items)
            clone.AddItem(item);

        return clone;
    }

    public Order DeepCopy()
    {
        var clone = new Order(CustomerId);
        clone.Priority = Priority;
        clone.DeliveryNotes = DeliveryNotes;

        foreach (var item in _items)
            clone.AddItem(new OrderItem(item.ProductName, item.Quantity, item.UnitPrice, item.Weight));

        return clone;
    }

    public override string ToString()
    {
        return $"Order {Id}: {_items.Count} items, Total: {GetTotalPrice():C}, Priority: {Priority}, Status: {Status}";
    }
}
