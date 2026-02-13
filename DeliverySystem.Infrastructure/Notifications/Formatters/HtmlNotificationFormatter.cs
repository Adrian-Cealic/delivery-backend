using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces.Notifications;

namespace DeliverySystem.Infrastructure.Notifications.Formatters;

public class HtmlNotificationFormatter : INotificationFormatter
{
    public string FormatOrderCreated(Order order, Customer customer)
    {
        return $"<div class='notification'>" +
               $"<h2>Order Created</h2>" +
               $"<p>Dear {customer.Name},</p>" +
               $"<p>Your order <strong>#{order.Id}</strong> has been placed.</p>" +
               $"<ul>" +
               $"<li>Items: {order.Items.Count}</li>" +
               $"<li>Total: {order.GetTotalPrice():C}</li>" +
               $"<li>Weight: {order.GetTotalWeight()}kg</li>" +
               $"</ul></div>";
    }

    public string FormatOrderStatusChanged(Order order, Customer customer)
    {
        return $"<div class='notification'>" +
               $"<h2>Order Status Update</h2>" +
               $"<p>Dear {customer.Name},</p>" +
               $"<p>Order <strong>#{order.Id}</strong> is now <em>{order.Status}</em>.</p>" +
               $"</div>";
    }

    public string FormatDeliveryAssigned(Delivery delivery, Customer customer, Courier courier)
    {
        return $"<div class='notification'>" +
               $"<h2>Delivery Assigned</h2>" +
               $"<p>Dear {customer.Name},</p>" +
               $"<p>Delivery <strong>#{delivery.Id}</strong> has been assigned.</p>" +
               $"<ul>" +
               $"<li>Courier: {courier.Name} ({courier.VehicleType})</li>" +
               $"<li>Distance: {delivery.DistanceKm}km</li>" +
               $"<li>ETA: {delivery.EstimatedDeliveryTime}</li>" +
               $"</ul></div>";
    }

    public string FormatDeliveryStatusChanged(Delivery delivery, Customer customer)
    {
        return $"<div class='notification'>" +
               $"<h2>Delivery Update</h2>" +
               $"<p>Dear {customer.Name},</p>" +
               $"<p>Delivery <strong>#{delivery.Id}</strong> is now <em>{delivery.Status}</em>.</p>" +
               $"</div>";
    }

    public string FormatDeliveryCompleted(Delivery delivery, Customer customer)
    {
        return $"<div class='notification'>" +
               $"<h2>Delivery Completed!</h2>" +
               $"<p>Dear {customer.Name},</p>" +
               $"<p>Delivery <strong>#{delivery.Id}</strong> was completed at {delivery.DeliveredAt}.</p>" +
               $"<p><em>Thank you for using our delivery service!</em></p>" +
               $"</div>";
    }
}
