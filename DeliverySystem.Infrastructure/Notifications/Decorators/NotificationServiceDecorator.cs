using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Infrastructure.Notifications.Decorators;

public abstract class NotificationServiceDecorator : INotificationService
{
    protected readonly INotificationService Inner;

    protected NotificationServiceDecorator(INotificationService inner)
    {
        Inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public virtual void NotifyOrderCreated(Order order, Customer customer)
        => Inner.NotifyOrderCreated(order, customer);

    public virtual void NotifyOrderStatusChanged(Order order, Customer customer)
        => Inner.NotifyOrderStatusChanged(order, customer);

    public virtual void NotifyDeliveryAssigned(Delivery delivery, Customer customer, Courier courier)
        => Inner.NotifyDeliveryAssigned(delivery, customer, courier);

    public virtual void NotifyDeliveryStatusChanged(Delivery delivery, Customer customer)
        => Inner.NotifyDeliveryStatusChanged(delivery, customer);

    public virtual void NotifyDeliveryCompleted(Delivery delivery, Customer customer)
        => Inner.NotifyDeliveryCompleted(delivery, customer);
}
