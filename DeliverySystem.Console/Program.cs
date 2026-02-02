using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.ValueObjects;
using DeliverySystem.Infrastructure.Notifications;
using DeliverySystem.Infrastructure.Repositories;
using DeliverySystem.Interfaces;
using DeliverySystem.Services;

namespace DeliverySystem.Console;

internal class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║     DELIVERY MANAGEMENT SYSTEM - LABORATOR 1                ║");
        System.Console.WriteLine("║     OOP Fundamentals + SOLID Principles                     ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        System.Console.WriteLine();

        var (orderService, deliveryService, customerRepository, courierRepository) = WireUpDependencies();

        DemoEncapsulationAndInheritance(customerRepository, courierRepository);

        DemoPolymorphism(courierRepository);

        DemoFullWorkflow(orderService, deliveryService, customerRepository, courierRepository);

        System.Console.WriteLine();
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║                    DEMO COMPLETED                            ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
    }

    static (OrderService, DeliveryService, ICustomerRepository, ICourierRepository) WireUpDependencies()
    {
        System.Console.WriteLine("=== WIRING DEPENDENCIES (DIP - Dependency Inversion) ===");
        System.Console.WriteLine();

        ICustomerRepository customerRepository = new InMemoryCustomerRepository();
        IOrderRepository orderRepository = new InMemoryOrderRepository();
        ICourierRepository courierRepository = new InMemoryCourierRepository();
        IDeliveryRepository deliveryRepository = new InMemoryDeliveryRepository();
        INotificationService notificationService = new ConsoleNotificationService();

        var orderService = new OrderService(
            orderRepository,
            customerRepository,
            notificationService
        );

        var deliveryService = new DeliveryService(
            deliveryRepository,
            orderRepository,
            courierRepository,
            customerRepository,
            notificationService
        );

        System.Console.WriteLine("All dependencies wired via Constructor Injection.");
        System.Console.WriteLine("Services depend on INTERFACES, not concrete implementations.");
        System.Console.WriteLine();

        return (orderService, deliveryService, customerRepository, courierRepository);
    }

    static void DemoEncapsulationAndInheritance(ICustomerRepository customerRepository, ICourierRepository courierRepository)
    {
        System.Console.WriteLine("=== DEMO: ENCAPSULATION + INHERITANCE ===");
        System.Console.WriteLine();

        System.Console.WriteLine("Creating Customer (inherits from EntityBase):");
        var address = new Address("Str. Stefan cel Mare 1", "Chisinau", "MD-2001", "Moldova");
        var customer = new Customer("Ion Popescu", "ion@email.com", "+373-69-123456", address);
        customerRepository.Add(customer);
        System.Console.WriteLine($"  {customer}");
        System.Console.WriteLine($"  Address: {customer.Address}");
        System.Console.WriteLine($"  ID (from EntityBase): {customer.Id}");
        System.Console.WriteLine();

        System.Console.WriteLine("Creating BikeCourier (inherits from Courier -> EntityBase):");
        var bikeCourier = new BikeCourier("Vasile Biciclist", "+373-69-111111");
        courierRepository.Add(bikeCourier);
        System.Console.WriteLine($"  {bikeCourier}");
        System.Console.WriteLine();

        System.Console.WriteLine("Creating CarCourier (inherits from Courier -> EntityBase):");
        var carCourier = new CarCourier("Mihai Sofer", "+373-69-222222", "ABC-123");
        courierRepository.Add(carCourier);
        System.Console.WriteLine($"  {carCourier}");
        System.Console.WriteLine();
    }

    static void DemoPolymorphism(ICourierRepository courierRepository)
    {
        System.Console.WriteLine("=== DEMO: POLYMORPHISM ===");
        System.Console.WriteLine();

        var allCouriers = courierRepository.GetAll().ToList();

        System.Console.WriteLine("Iterating over List<Courier> (polymorphic behavior):");
        System.Console.WriteLine();

        foreach (Courier courier in allCouriers)
        {
            System.Console.WriteLine($"  Courier: {courier.Name}");
            System.Console.WriteLine($"    Type: {courier.GetType().Name}");
            System.Console.WriteLine($"    VehicleType: {courier.VehicleType}");
            System.Console.WriteLine($"    MaxWeight: {courier.MaxWeight}kg");

            decimal testDistance = 10.0m;
            var deliveryTime = courier.CalculateDeliveryTime(testDistance);
            System.Console.WriteLine($"    Delivery Time for {testDistance}km: {deliveryTime.TotalMinutes} minutes");
            System.Console.WriteLine();
        }

        System.Console.WriteLine("Polymorphism demonstrated:");
        System.Console.WriteLine("  - Same method call (CalculateDeliveryTime) produces different results");
        System.Console.WriteLine("  - BikeCourier: 3 min/km = 30 minutes for 10km");
        System.Console.WriteLine("  - CarCourier: 1.5 min/km = 15 minutes for 10km");
        System.Console.WriteLine();
    }

    static void DemoFullWorkflow(
        OrderService orderService,
        DeliveryService deliveryService,
        ICustomerRepository customerRepository,
        ICourierRepository courierRepository)
    {
        System.Console.WriteLine("=== DEMO: FULL DELIVERY WORKFLOW ===");
        System.Console.WriteLine();

        var customer = customerRepository.GetAll().First();
        var couriers = courierRepository.GetAll().ToList();

        System.Console.WriteLine("Step 1: Creating Order with Items");
        var items = new List<OrderItem>
        {
            new OrderItem("Laptop Dell XPS 15", 1, 1500.00m, 2.0m),
            new OrderItem("Mouse Wireless", 2, 25.00m, 0.1m),
            new OrderItem("USB-C Hub", 1, 50.00m, 0.2m)
        };

        var order = orderService.CreateOrder(customer.Id, items);
        System.Console.WriteLine($"Order created: {order}");
        System.Console.WriteLine();

        System.Console.WriteLine("Step 2: Confirming Order");
        orderService.ConfirmOrder(order.Id);

        System.Console.WriteLine("Step 3: Processing Order");
        orderService.StartProcessing(order.Id);

        System.Console.WriteLine("Step 4: Mark Ready for Delivery");
        orderService.MarkReadyForDelivery(order.Id);

        System.Console.WriteLine();
        System.Console.WriteLine("Step 5: Finding suitable courier (polymorphic selection)");
        var orderWeight = order.GetTotalWeight();
        System.Console.WriteLine($"  Order weight: {orderWeight}kg");

        Courier? selectedCourier = null;
        foreach (var courier in couriers)
        {
            System.Console.WriteLine($"  Checking {courier.Name} ({courier.GetType().Name}):");
            System.Console.WriteLine($"    MaxWeight: {courier.MaxWeight}kg, CanCarry: {courier.CanCarry(orderWeight)}");

            if (courier.IsAvailable && courier.CanCarry(orderWeight))
            {
                selectedCourier = courier;
                System.Console.WriteLine($"    Selected!");
                break;
            }
        }

        if (selectedCourier == null)
        {
            System.Console.WriteLine("  No suitable courier found!");
            return;
        }

        System.Console.WriteLine();
        System.Console.WriteLine("Step 6: Assigning Delivery");
        decimal distance = 5.5m;
        var delivery = deliveryService.AssignCourierToOrder(order.Id, selectedCourier.Id, distance);

        System.Console.WriteLine();
        System.Console.WriteLine("Step 7: Delivery Progress");
        deliveryService.MarkPickedUp(delivery.Id);
        deliveryService.MarkInTransit(delivery.Id);
        deliveryService.MarkDelivered(delivery.Id);

        System.Console.WriteLine();
        System.Console.WriteLine("=== SOLID PRINCIPLES DEMONSTRATED ===");
        System.Console.WriteLine();
        System.Console.WriteLine("SRP: Each class has single responsibility");
        System.Console.WriteLine("     - OrderService manages orders only");
        System.Console.WriteLine("     - DeliveryService manages deliveries only");
        System.Console.WriteLine();
        System.Console.WriteLine("OCP: Courier hierarchy is open for extension");
        System.Console.WriteLine("     - Can add TruckCourier without modifying existing code");
        System.Console.WriteLine();
        System.Console.WriteLine("LSP: BikeCourier/CarCourier substitute Courier");
        System.Console.WriteLine("     - Used interchangeably in courier selection loop");
        System.Console.WriteLine();
        System.Console.WriteLine("ISP: Small, focused interfaces");
        System.Console.WriteLine("     - IOrderRepository, ICourierRepository, etc.");
        System.Console.WriteLine();
        System.Console.WriteLine("DIP: Services depend on interfaces");
        System.Console.WriteLine("     - OrderService depends on IOrderRepository, not InMemoryOrderRepository");
    }
}
