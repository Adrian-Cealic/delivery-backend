using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Builders;

public class StandardOrderBuilder : IOrderBuilder
{
    private Guid _customerId;
    private readonly List<OrderItem> _items = new();
    private OrderPriority _priority = OrderPriority.Normal;
    private string? _deliveryNotes;

    public IOrderBuilder SetCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));

        _customerId = customerId;
        return this;
    }

    public IOrderBuilder AddItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        _items.Add(item);
        return this;
    }

    public IOrderBuilder SetPriority(OrderPriority priority)
    {
        _priority = priority;
        return this;
    }

    public IOrderBuilder SetDeliveryNotes(string? notes)
    {
        _deliveryNotes = notes;
        return this;
    }

    public IOrderBuilder Reset()
    {
        _customerId = Guid.Empty;
        _items.Clear();
        _priority = OrderPriority.Normal;
        _deliveryNotes = null;
        return this;
    }

    public Order Build()
    {
        if (_customerId == Guid.Empty)
            throw new InvalidOperationException("Customer ID must be set before building an order.");

        if (_items.Count == 0)
            throw new InvalidOperationException("Order must have at least one item.");

        var order = new Order(_customerId);
        order.SetPriority(_priority);

        if (_deliveryNotes != null)
            order.SetDeliveryNotes(_deliveryNotes);

        foreach (var item in _items)
            order.AddItem(item);

        return order;
    }
}
