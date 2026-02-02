using DeliverySystem.Domain.Entities;

namespace DeliverySystem.Interfaces;

public interface INotificationService
{
    void NotifyOrderCreated(Order order, Customer customer);
    void NotifyOrderStatusChanged(Order order, Customer customer);
    void NotifyDeliveryAssigned(Delivery delivery, Customer customer, Courier courier);
    void NotifyDeliveryStatusChanged(Delivery delivery, Customer customer);
    void NotifyDeliveryCompleted(Delivery delivery, Customer customer);
}
