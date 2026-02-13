using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;
using DeliverySystem.Interfaces.Notifications;

namespace DeliverySystem.Infrastructure.Notifications;

public class ConsoleNotificationService : INotificationService
{
    private readonly INotificationSender _sender;
    private readonly INotificationFormatter _formatter;

    public ConsoleNotificationService(INotificationFactory notificationFactory)
    {
        if (notificationFactory == null)
            throw new ArgumentNullException(nameof(notificationFactory));

        _sender = notificationFactory.CreateSender();
        _formatter = notificationFactory.CreateFormatter();
    }

    public void NotifyOrderCreated(Order order, Customer customer)
    {
        var body = _formatter.FormatOrderCreated(order, customer);
        _sender.Send(customer.Email, "Order Created", body);
    }

    public void NotifyOrderStatusChanged(Order order, Customer customer)
    {
        var body = _formatter.FormatOrderStatusChanged(order, customer);
        _sender.Send(customer.Email, "Order Status Update", body);
    }

    public void NotifyDeliveryAssigned(Delivery delivery, Customer customer, Courier courier)
    {
        var body = _formatter.FormatDeliveryAssigned(delivery, customer, courier);
        _sender.Send(customer.Email, "Delivery Assigned", body);
    }

    public void NotifyDeliveryStatusChanged(Delivery delivery, Customer customer)
    {
        var body = _formatter.FormatDeliveryStatusChanged(delivery, customer);
        _sender.Send(customer.Email, "Delivery Status Update", body);
    }

    public void NotifyDeliveryCompleted(Delivery delivery, Customer customer)
    {
        var body = _formatter.FormatDeliveryCompleted(delivery, customer);
        _sender.Send(customer.Email, "Delivery Completed", body);
    }
}
