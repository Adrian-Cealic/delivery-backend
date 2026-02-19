using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Builders;

public interface IOrderBuilder
{
    IOrderBuilder SetCustomerId(Guid customerId);
    IOrderBuilder AddItem(OrderItem item);
    IOrderBuilder SetPriority(OrderPriority priority);
    IOrderBuilder SetDeliveryNotes(string? notes);
    IOrderBuilder Reset();
    Order Build();
}
