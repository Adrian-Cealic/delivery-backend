using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Infrastructure.Notifications.Decorators;

public sealed class LoggingNotificationDecorator : NotificationServiceDecorator
{
    private readonly Action<string> _logAction;

    public LoggingNotificationDecorator(INotificationService inner)
        : base(inner)
    {
        _logAction = message => Console.WriteLine($"[LOG {DateTime.UtcNow:O}] {message}");
    }

    public LoggingNotificationDecorator(INotificationService inner, Action<string> logAction)
        : base(inner)
    {
        _logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
    }

    public override void NotifyOrderCreated(Order order, Customer customer)
    {
        _logAction($"NotifyOrderCreated — Order {order.Id}, Customer {customer.Name}");
        base.NotifyOrderCreated(order, customer);
    }

    public override void NotifyOrderStatusChanged(Order order, Customer customer)
    {
        _logAction($"NotifyOrderStatusChanged — Order {order.Id}, Status {order.Status}");
        base.NotifyOrderStatusChanged(order, customer);
    }

    public override void NotifyDeliveryAssigned(Delivery delivery, Customer customer, Courier courier)
    {
        _logAction($"NotifyDeliveryAssigned — Delivery {delivery.Id}, Courier {courier.Name}");
        base.NotifyDeliveryAssigned(delivery, customer, courier);
    }

    public override void NotifyDeliveryStatusChanged(Delivery delivery, Customer customer)
    {
        _logAction($"NotifyDeliveryStatusChanged — Delivery {delivery.Id}, Status {delivery.Status}");
        base.NotifyDeliveryStatusChanged(delivery, customer);
    }

    public override void NotifyDeliveryCompleted(Delivery delivery, Customer customer)
    {
        _logAction($"NotifyDeliveryCompleted — Delivery {delivery.Id}");
        base.NotifyDeliveryCompleted(delivery, customer);
    }
}
