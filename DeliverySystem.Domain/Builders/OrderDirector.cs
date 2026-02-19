using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Builders;

public class OrderDirector
{
    private readonly IOrderBuilder _builder;

    public OrderDirector(IOrderBuilder builder)
    {
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    public Order BuildStandardOrder(Guid customerId, IEnumerable<OrderItem> items)
    {
        _builder.Reset()
            .SetCustomerId(customerId)
            .SetPriority(OrderPriority.Normal);

        foreach (var item in items)
            _builder.AddItem(item);

        return _builder.Build();
    }

    public Order BuildExpressOrder(Guid customerId, IEnumerable<OrderItem> items, string? notes = null)
    {
        _builder.Reset()
            .SetCustomerId(customerId)
            .SetPriority(OrderPriority.Express)
            .SetDeliveryNotes(notes ?? "Express delivery requested");

        foreach (var item in items)
            _builder.AddItem(item);

        return _builder.Build();
    }

    public Order BuildEconomyOrder(Guid customerId, IEnumerable<OrderItem> items)
    {
        _builder.Reset()
            .SetCustomerId(customerId)
            .SetPriority(OrderPriority.Economy)
            .SetDeliveryNotes("Economy shipping - no rush");

        foreach (var item in items)
            _builder.AddItem(item);

        return _builder.Build();
    }
}
