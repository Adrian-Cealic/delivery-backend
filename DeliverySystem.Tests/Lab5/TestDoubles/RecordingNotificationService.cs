using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Tests.Lab5.TestDoubles;

public sealed class RecordingNotificationService : INotificationService
{
    public List<string> RecordedCalls { get; } = new();

    public void NotifyOrderCreated(Order order, Customer customer)
        => RecordedCalls.Add($"OrderCreated:{order.Id}");

    public void NotifyOrderStatusChanged(Order order, Customer customer)
        => RecordedCalls.Add($"OrderStatusChanged:{order.Id}");

    public void NotifyDeliveryAssigned(Delivery delivery, Customer customer, Courier courier)
        => RecordedCalls.Add($"DeliveryAssigned:{delivery.Id}");

    public void NotifyDeliveryStatusChanged(Delivery delivery, Customer customer)
        => RecordedCalls.Add($"DeliveryStatusChanged:{delivery.Id}");

    public void NotifyDeliveryCompleted(Delivery delivery, Customer customer)
        => RecordedCalls.Add($"DeliveryCompleted:{delivery.Id}");
}
