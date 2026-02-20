using DeliverySystem.Domain.Builders;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Factories;
using DeliverySystem.Domain.ValueObjects;
using DeliverySystem.Infrastructure.Configuration;
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
        System.Console.WriteLine("║     DELIVERY MANAGEMENT SYSTEM - LABORATOR 3                ║");
        System.Console.WriteLine("║     Creational Patterns: Builder, Prototype, Singleton       ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        System.Console.WriteLine();

        var (orderService, deliveryService, customerRepository, courierRepository) = WireUpDependencies();

        DemoSingleton();

        DemoBuilder(customerRepository);

        DemoPrototype(customerRepository, orderService);

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

        var config = DeliverySystemConfiguration.Instance;

        var orderService = new OrderService(orderRepository, customerRepository, notificationService);
        var deliveryService = new DeliveryService(
            deliveryRepository, orderRepository, courierRepository, customerRepository, notificationService, config);

        System.Console.WriteLine("Dependencies wired with Singleton configuration.");
        System.Console.WriteLine();

        return (orderService, deliveryService, customerRepository, courierRepository);
    }

    static void DemoSingleton()
    {
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║              SINGLETON PATTERN DEMO                          ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        System.Console.WriteLine();

        var config1 = DeliverySystemConfiguration.Instance;
        var config2 = DeliverySystemConfiguration.Instance;

        System.Console.WriteLine($"  config1 == config2: {ReferenceEquals(config1, config2)}");
        System.Console.WriteLine($"  Default settings: {config1}");

        config1.SetMaxDeliveryDistance(150.0m);
        config1.SetDefaultCurrency("EUR");
        System.Console.WriteLine($"  After update via config1: {config1}");
        System.Console.WriteLine($"  Read via config2:         {config2}");
        System.Console.WriteLine($"  Same values? {config2.MaxDeliveryDistanceKm == 150.0m && config2.DefaultCurrency == "EUR"}");

        config1.SetMaxDeliveryDistance(100.0m);
        config1.SetDefaultCurrency("MDL");

        System.Console.WriteLine();
        System.Console.WriteLine("  Thread-safety test (10 concurrent accesses):");
        var instances = new DeliverySystemConfiguration[10];
        Parallel.For(0, 10, i => { instances[i] = DeliverySystemConfiguration.Instance; });
        var allSame = instances.All(inst => ReferenceEquals(inst, instances[0]));
        System.Console.WriteLine($"  All instances identical: {allSame}");

        System.Console.WriteLine();
        System.Console.WriteLine("Singleton benefits:");
        System.Console.WriteLine("  - Single configuration instance shared across all services");
        System.Console.WriteLine("  - Thread-safe via Lazy<T> and lock for updates");
        System.Console.WriteLine("  - Changes visible instantly to all consumers");
        System.Console.WriteLine();
    }

    static void DemoBuilder(ICustomerRepository customerRepository)
    {
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║              BUILDER PATTERN DEMO                            ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        System.Console.WriteLine();

        var address = new Address("Str. Stefan cel Mare 1", "Chisinau", "MD-2001", "Moldova");
        var customer = new Customer("Ion Popescu", "ion@email.com", "+373-69-123456", address);
        customerRepository.Add(customer);

        System.Console.WriteLine("1. Fluent Builder (step-by-step):");
        var builder = new StandardOrderBuilder();
        var expressOrder = builder
            .SetCustomerId(customer.Id)
            .AddItem(new OrderItem("Gaming Laptop", 1, 1299.99m, 3.0m))
            .AddItem(new OrderItem("Gaming Mouse", 1, 79.99m, 0.2m))
            .SetPriority(OrderPriority.Express)
            .SetDeliveryNotes("Fragile electronics - handle with care")
            .Build();

        System.Console.WriteLine($"  Built express order: {expressOrder}");
        System.Console.WriteLine($"  Notes: {expressOrder.DeliveryNotes}");
        System.Console.WriteLine();

        System.Console.WriteLine("2. Director with presets:");
        var director = new OrderDirector(new StandardOrderBuilder());

        var standardItems = new[]
        {
            new OrderItem("Book", 3, 12.99m, 0.5m),
            new OrderItem("Notebook", 5, 3.50m, 0.2m)
        };

        var standardOrder = director.BuildStandardOrder(customer.Id, standardItems);
        System.Console.WriteLine($"  Standard order: {standardOrder}");

        var expressItems = new[]
        {
            new OrderItem("Medicine", 1, 25.00m, 0.1m)
        };

        var expressDirectorOrder = director.BuildExpressOrder(customer.Id, expressItems, "Urgent medical supplies");
        System.Console.WriteLine($"  Express order:  {expressDirectorOrder}");
        System.Console.WriteLine($"  Express notes:  {expressDirectorOrder.DeliveryNotes}");

        var economyItems = new[]
        {
            new OrderItem("Office Supplies", 10, 5.00m, 0.3m)
        };

        var economyOrder = director.BuildEconomyOrder(customer.Id, economyItems);
        System.Console.WriteLine($"  Economy order:  {economyOrder}");

        System.Console.WriteLine();
        System.Console.WriteLine("Builder benefits:");
        System.Console.WriteLine("  - Complex object construction separated from representation");
        System.Console.WriteLine("  - Fluent API makes code readable and self-documenting");
        System.Console.WriteLine("  - Director provides reusable presets (SRP)");
        System.Console.WriteLine("  - Easy to add new builder types without changing Order (OCP)");
        System.Console.WriteLine();
    }

    static void DemoPrototype(ICustomerRepository customerRepository, OrderService orderService)
    {
        System.Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        System.Console.WriteLine("║              PROTOTYPE PATTERN DEMO                          ║");
        System.Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        System.Console.WriteLine();

        var customers = customerRepository.GetAll().ToList();
        var customer = customers.First();

        var items = new[]
        {
            new OrderItem("Wireless Earbuds", 2, 45.00m, 0.1m),
            new OrderItem("USB-C Cable", 3, 8.99m, 0.05m)
        };
        var original = orderService.CreateOrder(customer.Id, items, OrderPriority.Express, "Gift wrapping requested");

        System.Console.WriteLine($"  Original: {original}");
        System.Console.WriteLine($"  Original ID: {original.Id}");
        System.Console.WriteLine();

        System.Console.WriteLine("1. Shallow Clone (shares item references):");
        var shallow = original.Clone();
        System.Console.WriteLine($"  Clone ID: {shallow.Id}");
        System.Console.WriteLine($"  Same customer? {original.CustomerId == shallow.CustomerId}");
        System.Console.WriteLine($"  Same priority? {original.Priority == shallow.Priority}");
        System.Console.WriteLine($"  Same items[0] reference? {ReferenceEquals(original.Items[0], shallow.Items[0])}");
        System.Console.WriteLine();

        System.Console.WriteLine("2. Deep Copy (new item instances):");
        var deep = original.DeepCopy();
        System.Console.WriteLine($"  DeepCopy ID: {deep.Id}");
        System.Console.WriteLine($"  Same items[0] reference? {ReferenceEquals(original.Items[0], deep.Items[0])}");
        System.Console.WriteLine($"  Equal item values? {original.Items[0].ProductName == deep.Items[0].ProductName}");
        System.Console.WriteLine();

        System.Console.WriteLine("3. Independence test:");
        deep.AddItem(new OrderItem("Extra Charger", 1, 15.0m, 0.1m));
        System.Console.WriteLine($"  Original items: {original.Items.Count}");
        System.Console.WriteLine($"  DeepCopy items: {deep.Items.Count}");
        System.Console.WriteLine($"  Original unaffected: {original.Items.Count == 2}");

        System.Console.WriteLine();
        System.Console.WriteLine("Prototype benefits:");
        System.Console.WriteLine("  - Fast object creation by cloning instead of constructing");
        System.Console.WriteLine("  - Re-order scenario: duplicate an order in one call");
        System.Console.WriteLine("  - Shallow vs deep copy for different isolation needs");
        System.Console.WriteLine("  - Decouples client from concrete class construction details");
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

        var bikeFactory = CourierFactoryProvider.GetFactory(VehicleType.Bicycle);
        var bikeCourier = bikeFactory.CreateAndValidate(new CourierCreationParams("Vasile Biciclist", "+373-69-111111"));
        courierRepository.Add(bikeCourier);

        var customers = customerRepository.GetAll().ToList();
        var customer = customers.First();

        var director = new OrderDirector(new StandardOrderBuilder());
        var order = director.BuildExpressOrder(customer.Id, new[]
        {
            new OrderItem("Phone Case", 2, 15.00m, 0.05m)
        }, "Deliver ASAP");

        var savedOrder = orderService.RegisterOrder(order);
        orderService.ConfirmOrder(savedOrder.Id);
        orderService.StartProcessing(savedOrder.Id);
        orderService.MarkReadyForDelivery(savedOrder.Id);

        var selectedCourier = deliveryService.FindAvailableCourier(savedOrder.GetTotalWeight());
        if (selectedCourier != null)
        {
            System.Console.WriteLine($"Selected courier: {selectedCourier}");
            var delivery = deliveryService.AssignCourierToOrder(savedOrder.Id, selectedCourier.Id, 3.5m);
            deliveryService.MarkPickedUp(delivery.Id);
            deliveryService.MarkInTransit(delivery.Id);
            deliveryService.MarkDelivered(delivery.Id);
        }
    }
}
