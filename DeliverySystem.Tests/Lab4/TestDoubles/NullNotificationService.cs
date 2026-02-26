using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Tests.Lab4.TestDoubles;

public sealed class NullNotificationService : INotificationService
{
    public void NotifyOrderCreated(Order order, Customer customer) { }

    public void NotifyOrderStatusChanged(Order order, Customer customer) { }

    public void NotifyDeliveryAssigned(Delivery delivery, Customer customer, Courier courier) { }

    public void NotifyDeliveryStatusChanged(Delivery delivery, Customer customer) { }

    public void NotifyDeliveryCompleted(Delivery delivery, Customer customer) { }
}
