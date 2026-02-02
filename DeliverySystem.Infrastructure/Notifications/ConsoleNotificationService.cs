using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Infrastructure.Notifications;

public class ConsoleNotificationService : INotificationService
{
    public void NotifyOrderCreated(Order order, Customer customer)
    {
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    ORDER CREATED                             ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        Console.WriteLine($"║ To: {customer.Name} ({customer.Email})");
        Console.WriteLine($"║ Order ID: {order.Id}");
        Console.WriteLine($"║ Items: {order.Items.Count}");
        Console.WriteLine($"║ Total: {order.GetTotalPrice():C}");
        Console.WriteLine($"║ Total Weight: {order.GetTotalWeight()}kg");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    }

    public void NotifyOrderStatusChanged(Order order, Customer customer)
    {
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  ORDER STATUS UPDATE                         ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        Console.WriteLine($"║ To: {customer.Name} ({customer.Email})");
        Console.WriteLine($"║ Order ID: {order.Id}");
        Console.WriteLine($"║ New Status: {order.Status}");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    }

    public void NotifyDeliveryAssigned(Delivery delivery, Customer customer, Courier courier)
    {
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                   DELIVERY ASSIGNED                          ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        Console.WriteLine($"║ To: {customer.Name} ({customer.Email})");
        Console.WriteLine($"║ Delivery ID: {delivery.Id}");
        Console.WriteLine($"║ Courier: {courier.Name} ({courier.VehicleType})");
        Console.WriteLine($"║ Distance: {delivery.DistanceKm}km");
        Console.WriteLine($"║ Estimated Time: {delivery.EstimatedDeliveryTime}");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    }

    public void NotifyDeliveryStatusChanged(Delivery delivery, Customer customer)
    {
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                DELIVERY STATUS UPDATE                        ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        Console.WriteLine($"║ To: {customer.Name} ({customer.Email})");
        Console.WriteLine($"║ Delivery ID: {delivery.Id}");
        Console.WriteLine($"║ New Status: {delivery.Status}");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    }

    public void NotifyDeliveryCompleted(Delivery delivery, Customer customer)
    {
        Console.WriteLine();
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                  DELIVERY COMPLETED                          ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        Console.WriteLine($"║ To: {customer.Name} ({customer.Email})");
        Console.WriteLine($"║ Delivery ID: {delivery.Id}");
        Console.WriteLine($"║ Delivered At: {delivery.DeliveredAt}");
        Console.WriteLine("║ Thank you for using our delivery service!");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    }
}
