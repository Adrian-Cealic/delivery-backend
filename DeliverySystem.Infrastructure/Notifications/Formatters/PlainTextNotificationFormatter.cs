using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces.Notifications;

namespace DeliverySystem.Infrastructure.Notifications.Formatters;

public class PlainTextNotificationFormatter : INotificationFormatter
{
    public string FormatOrderCreated(Order order, Customer customer)
    {
        return $"║  Order #{order.Id} has been created.\n" +
               $"║  Items: {order.Items.Count}\n" +
               $"║  Total: {order.GetTotalPrice():C}\n" +
               $"║  Weight: {order.GetTotalWeight()}kg";
    }

    public string FormatOrderStatusChanged(Order order, Customer customer)
    {
        return $"║  Order #{order.Id} status changed to {order.Status}.";
    }

    public string FormatDeliveryAssigned(Delivery delivery, Customer customer, Courier courier)
    {
        return $"║  Delivery #{delivery.Id} assigned.\n" +
               $"║  Courier: {courier.Name} ({courier.VehicleType})\n" +
               $"║  Distance: {delivery.DistanceKm}km\n" +
               $"║  ETA: {delivery.EstimatedDeliveryTime}";
    }

    public string FormatDeliveryStatusChanged(Delivery delivery, Customer customer)
    {
        return $"║  Delivery #{delivery.Id} status: {delivery.Status}.";
    }

    public string FormatDeliveryCompleted(Delivery delivery, Customer customer)
    {
        return $"║  Delivery #{delivery.Id} completed at {delivery.DeliveredAt}.\n" +
               $"║  Thank you for using our service!";
    }
}
