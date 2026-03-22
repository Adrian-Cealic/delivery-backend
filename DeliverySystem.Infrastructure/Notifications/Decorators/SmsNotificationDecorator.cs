using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Infrastructure.Notifications.Decorators;

public sealed class SmsNotificationDecorator : NotificationServiceDecorator
{
    private readonly Action<string, string> _smsAction;

    public SmsNotificationDecorator(INotificationService inner)
        : base(inner)
    {
        _smsAction = (phone, message) =>
            Console.WriteLine($"[SMS → {phone}] {message}");
    }

    public SmsNotificationDecorator(INotificationService inner, Action<string, string> smsAction)
        : base(inner)
    {
        _smsAction = smsAction ?? throw new ArgumentNullException(nameof(smsAction));
    }

    public override void NotifyOrderCreated(Order order, Customer customer)
    {
        _smsAction(customer.Phone, $"Order {order.Id} created.");
        base.NotifyOrderCreated(order, customer);
    }

    public override void NotifyOrderStatusChanged(Order order, Customer customer)
    {
        _smsAction(customer.Phone, $"Order {order.Id} status: {order.Status}.");
        base.NotifyOrderStatusChanged(order, customer);
    }

    public override void NotifyDeliveryAssigned(Delivery delivery, Customer customer, Courier courier)
    {
        _smsAction(customer.Phone, $"Delivery {delivery.Id} assigned to {courier.Name}.");
        base.NotifyDeliveryAssigned(delivery, customer, courier);
    }

    public override void NotifyDeliveryStatusChanged(Delivery delivery, Customer customer)
    {
        _smsAction(customer.Phone, $"Delivery {delivery.Id} status: {delivery.Status}.");
        base.NotifyDeliveryStatusChanged(delivery, customer);
    }

    public override void NotifyDeliveryCompleted(Delivery delivery, Customer customer)
    {
        _smsAction(customer.Phone, $"Delivery {delivery.Id} completed!");
        base.NotifyDeliveryCompleted(delivery, customer);
    }
}
