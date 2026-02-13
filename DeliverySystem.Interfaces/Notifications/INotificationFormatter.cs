using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Interfaces.Notifications;

public interface INotificationFormatter
{
    string FormatOrderCreated(Order order, Customer customer);
    string FormatOrderStatusChanged(Order order, Customer customer);
    string FormatDeliveryAssigned(Delivery delivery, Customer customer, Courier courier);
    string FormatDeliveryStatusChanged(Delivery delivery, Customer customer);
    string FormatDeliveryCompleted(Delivery delivery, Customer customer);
}
