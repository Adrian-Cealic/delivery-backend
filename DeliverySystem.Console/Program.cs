using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Factories;
using DeliverySystem.Domain.ValueObjects;
using DeliverySystem.Infrastructure.Notifications;
using DeliverySystem.Infrastructure.Notifications.Factories;
using DeliverySystem.Infrastructure.Repositories;
using DeliverySystem.Interfaces;
using DeliverySystem.Interfaces.Notifications;
using DeliverySystem.Services;

namespace DeliverySystem.Console;

internal class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║     DELIVERY MANAGEMENT SYSTEM - LABORATOR 2                ║");
        System.Console.WriteLine("║     Creational Patterns: Factory Method + Abstract Factory   ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        System.Console.WriteLine();

        var (orderService, deliveryService, customerRepository, courierRepository) = WireUpDependencies();

        DemoFactoryMethod(courierRepository);

        DemoAbstractFactory();

        DemoFullWorkflow(orderService, deliveryService, customerRepository, courierRepository);

        System.Console.WriteLine();
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║                    DEMO COMPLETED                            ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    }

    static (OrderService, DeliveryService, ICustomerRepository, ICourierRepository) WireUpDependencies()
    {
        System.Console.WriteLine("=== WIRING DEPENDENCIES ===");
        System.Console.WriteLine();

        ICustomerRepository customerRepository = new InMemoryCustomerRepository();
        IOrderRepository orderRepository = new InMemoryOrderRepository();
        ICourierRepository courierRepository = new InMemoryCourierRepository();
        IDeliveryRepository deliveryRepository = new InMemoryDeliveryRepository();

        INotificationFactory notificationFactory = new ConsoleNotificationFactory();
        INotificationService notificationService = new ConsoleNotificationService(notificationFactory);

        var orderService = new OrderService(orderRepository, customerRepository, notificationService);
        var deliveryService = new DeliveryService(
            deliveryRepository, orderRepository, courierRepository, customerRepository, notificationService);

        System.Console.WriteLine("Dependencies wired with Abstract Factory for notifications.");
        System.Console.WriteLine();

        return (orderService, deliveryService, customerRepository, courierRepository);
    }

    static void DemoFactoryMethod(ICourierRepository courierRepository)
    {
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║             FACTORY METHOD PATTERN DEMO                      ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        System.Console.WriteLine();

        System.Console.WriteLine("Creating couriers via Factory Method pattern:");
        System.Console.WriteLine("Each factory encapsulates the creation of a specific courier type.");
        System.Console.WriteLine();

        var bikeFactory = CourierFactoryProvider.GetFactory(VehicleType.Bicycle);
        var bikeParams = new CourierCreationParams("Vasile Biciclist", "+373-69-111111");
        var bikeCourier = bikeFactory.CreateAndValidate(bikeParams);
        courierRepository.Add(bikeCourier);
        System.Console.WriteLine($"  BikeCourierFactory created: {bikeCourier}");

        var carFactory = CourierFactoryProvider.GetFactory(VehicleType.Car);
        var carParams = new CourierCreationParams("Mihai Sofer", "+373-69-222222", licensePlate: "ABC-123");
        var carCourier = carFactory.CreateAndValidate(carParams);
        courierRepository.Add(carCourier);
        System.Console.WriteLine($"  CarCourierFactory created:  {carCourier}");

        var droneFactory = CourierFactoryProvider.GetFactory(VehicleType.Drone);
        var droneParams = new CourierCreationParams("Drona-X1", "+373-69-333333", maxFlightRangeKm: 15.0m);
        var droneCourier = droneFactory.CreateAndValidate(droneParams);
        courierRepository.Add(droneCourier);
        System.Console.WriteLine($"  DroneCourierFactory created: {droneCourier}");

        System.Console.WriteLine();
        System.Console.WriteLine("Polymorphic behavior via CalculateDeliveryTime (10km):");
        foreach (var courier in courierRepository.GetAll())
        {
            var time = courier.CalculateDeliveryTime(10.0m);
            System.Console.WriteLine($"  {courier.GetType().Name}: {time.TotalMinutes} minutes");
        }

        System.Console.WriteLine();
        System.Console.WriteLine("Factory Method benefits:");
        System.Console.WriteLine("  - New courier types added without modifying existing factories (OCP)");
        System.Console.WriteLine("  - Client code works with abstract CourierFactory, not concrete classes (DIP)");
        System.Console.WriteLine("  - Validation logic centralized in CreateAndValidate template method");
        System.Console.WriteLine();
    }

    static void DemoAbstractFactory()
    {
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║           ABSTRACT FACTORY PATTERN DEMO                      ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        System.Console.WriteLine();

        System.Console.WriteLine("Console Notification Factory (PlainText + Console):");
        INotificationFactory consoleFactory = new ConsoleNotificationFactory();
        var consoleSender = consoleFactory.CreateSender();
        var consoleFormatter = consoleFactory.CreateFormatter();
        System.Console.WriteLine($"  Sender type:    {consoleSender.GetType().Name}");
        System.Console.WriteLine($"  Formatter type: {consoleFormatter.GetType().Name}");
        consoleSender.Send("test@example.com", "Test", "Plain text notification body");

        System.Console.WriteLine();
        System.Console.WriteLine("Email Notification Factory (HTML + Email):");
        INotificationFactory emailFactory = new EmailNotificationFactory();
        var emailSender = emailFactory.CreateSender();
        var emailFormatter = emailFactory.CreateFormatter();
        System.Console.WriteLine($"  Sender type:    {emailSender.GetType().Name}");
        System.Console.WriteLine($"  Formatter type: {emailFormatter.GetType().Name}");
        emailSender.Send("test@example.com", "Test", "<h1>HTML notification body</h1>");

        System.Console.WriteLine();
        System.Console.WriteLine("Abstract Factory benefits:");
        System.Console.WriteLine("  - Families of related objects created together (sender + formatter)");
        System.Console.WriteLine("  - Switching notification channel = swap one factory");
        System.Console.WriteLine("  - Guaranteed consistency: Console sender always gets PlainText formatter");
        System.Console.WriteLine("  - Easy to add SMS factory without touching existing code (OCP)");
        System.Console.WriteLine();
    }

    static void DemoFullWorkflow(
        OrderService orderService,
        DeliveryService deliveryService,
        ICustomerRepository customerRepository,
        ICourierRepository courierRepository)
    {
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║              FULL DELIVERY WORKFLOW                          ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        System.Console.WriteLine();

        var address = new Address("Str. Stefan cel Mare 1", "Chisinau", "MD-2001", "Moldova");
        var customer = new Customer("Ion Popescu", "ion@email.com", "+373-69-123456", address);
        customerRepository.Add(customer);

        var items = new List<OrderItem>
        {
            new OrderItem("Wireless Earbuds", 1, 45.00m, 0.1m),
            new OrderItem("Phone Case", 2, 15.00m, 0.05m)
        };

        var order = orderService.CreateOrder(customer.Id, items);
        orderService.ConfirmOrder(order.Id);
        orderService.StartProcessing(order.Id);
        orderService.MarkReadyForDelivery(order.Id);

        var selectedCourier = deliveryService.FindAvailableCourier(order.GetTotalWeight());
        if (selectedCourier != null)
        {
            System.Console.WriteLine($"Selected courier via factory: {selectedCourier}");
            var delivery = deliveryService.AssignCourierToOrder(order.Id, selectedCourier.Id, 3.5m);
            deliveryService.MarkPickedUp(delivery.Id);
            deliveryService.MarkInTransit(delivery.Id);
            deliveryService.MarkDelivered(delivery.Id);
        }
    }
}
