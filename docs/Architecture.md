# Delivery Management System - Laborator 1

## Arhitectura Sistemului

### Diagrama de Clase UML
![Class Diagram](http://www.plantuml.com/plantuml/svg/tVdLc9s2EL7zV-ykh1KylDjT8UXjOpHlONXUadTITo8ZiIQtVCDAAKBdNfV_7wJ8gSTk8WRaHTQk9oFvv13sgm-1IcoUGYcLytk9Vfv1XhuafVlwovUFI3eKZFFkmOG0UYEPRJA7mlFhoFSHKTgDqCwgviIbqYiRCl6PokjvmMiJFSRWbW6MYpvC0GUixZr9TeG4r3IphVmbPe66kTyNopwkO9wTXlzIjDABV2RP1Qv4FgH-3B_ZaKNIYkp7eCcQ9P6caFop2d8PsExn8L5gabM0nTaPR55RPAovs8q-I_5aEK5juflzBvhHE_NmNEPckntK76n5hejtQqboewZMGCd8bAMocS8KbWRGlYd6Cr-RjM4A42Pizlt-h1TwwPpqK0VIf56mimo9qx_CJNQIYoG7ToDaTSaQW58TIKWlH_6aGovPqdebYoT30mPZaTm4MfVBB_Uc_Dj3gwjqVVHEpBeWrzzg96NKe-R-YVjBaH3FtDl14iUunHkaNSPD4oG1IaZAY2dXvviGihJD07mZwQU-XLOMetKbPO1L34RT4rzHSQ-GnwQM3aKObSwVGvs-5O0TzeQ9fZ5uCbCMKhb0IRDs0AgL_VoawleKJa7UU5qwjPCAyh-U3W1NXyecMovQS9sRrJRMi8QEj8YR_F4Qd2jbk1aFJJhxyEK4SjxDyTAXjr-8RTCBr9WGEyjqLSbw4ByO_id2eh1vIQvFntc4DjWIpZ7f4-kkG057DaxDwbd650f4TLcs4fR6n6OF9xLW_kD-OsRxR29BeFJwrL165NizEad4QIlI6K9ZY47kWNE6J6LXG-o44mDruBHkCYUFEQui1D5-6IL1u_qgSs_Zjg5TcPQchoa0wM9w8vI4zL63UdWiXascdQL4Tv6G44ioUF1dYfUKTVd2j8Dp--6Yjw8F3eLwY54A94D8Jww0JeKHGPNAvE-Omeau5LNWto7hEKkie2K8NLez_oSZa83uxMERs2LJjqY3-YEZU_s9OITwQjdgzL-AaIMrOMN8lltCD0yzWjmWJR8TSGoCJtCmaNSpFrWb6zrYwJEtFepwDyosxbUiQjNzUKNh5KDGJTaOnniQ_uomAqennwkv6Ed3Kzw76zSGtVGUmsDpWbi5NVheSWSG2wtkyEYWwqihWYf4-sKk3c5Iu5tXeePXJcK66U2sy4Lz2nbU2cILnIoi828GXqzVNah9l-KWqcxbwWGeoHcf-SdK0v2lVHW1NIKlGCw1SWu3sDXEOR3kx8HsHicP6YqK1AdRV1yrUFWYh6aqqCfAlBUTROI1Rw_GOUv2uOpFoyrrxyj6EffcUsXcMQFFsScxKfSW5TryvnpO_5m2N9eBwCVqqF6ew8F6Q3g9C9yqN4i6grZZW7QLmWGJMQvyFfIpE-YA95A33z3jafOhAjPYEh2VV_ZxjdrdBGdYqsLgl2Atnk7P2m8nnNOUS3GnwUhP7ldn6bpp040YBbdSdQV1cDMgVT1Yvx2VXkWV3ms7q-AnupRGQhrMn52BIG8b7gHG43nnZjceR7aqMqQKVw3VeKdtCgBFRKR4iPk-kyrfMp1FWMRgnfe38BMG_gQ-2dmSX-eU4gj6CTImXu0Ou_HS2_Vy7Lt5_fJk6OgtPhYZ_xc=)

### Structura Proiectului

```
DeliverySystem/
├── DeliverySystem.Domain/           # Entități și logică de business
│   ├── Entities/
│   │   ├── EntityBase.cs            # Clasă abstractă de bază
│   │   ├── Customer.cs              # Moștenește EntityBase
│   │   ├── Order.cs                 # Moștenește EntityBase
│   │   ├── OrderItem.cs             # Value object pentru item-uri
│   │   ├── Courier.cs               # Clasă abstractă pentru curieri
│   │   ├── BikeCourier.cs           # Moștenește Courier (polimorfism)
│   │   ├── CarCourier.cs            # Moștenește Courier (polimorfism)
│   │   └── Delivery.cs              # Moștenește EntityBase
│   ├── Enums/
│   │   ├── OrderStatus.cs
│   │   ├── DeliveryStatus.cs
│   │   └── VehicleType.cs
│   └── ValueObjects/
│       └── Address.cs               # Value object imutabil
│
├── DeliverySystem.Interfaces/       # Abstracții (DIP)
│   ├── ICustomerRepository.cs
│   ├── IOrderRepository.cs
│   ├── ICourierRepository.cs
│   ├── IDeliveryRepository.cs
│   └── INotificationService.cs
│
├── DeliverySystem.Services/         # Logică de aplicație (SRP)
│   ├── OrderService.cs              # Gestionare comenzi
│   └── DeliveryService.cs           # Gestionare livrări
│
├── DeliverySystem.Infrastructure/   # Implementări concrete
│   ├── Repositories/
│   │   ├── InMemoryCustomerRepository.cs
│   │   ├── InMemoryOrderRepository.cs
│   │   ├── InMemoryCourierRepository.cs
│   │   └── InMemoryDeliveryRepository.cs
│   └── Notifications/
│       └── ConsoleNotificationService.cs
│
└── DeliverySystem.Console/          # Aplicație demonstrativă
    └── Program.cs                   # DOAR wiring + demo
```

## Principii OOP Demonstrate

### 1. Encapsulare
- Toate câmpurile sunt `private`
- Acces prin proprietăți publice cu validare
- Colecții expuse ca `IReadOnlyList`

```csharp
public class Customer : EntityBase
{
    public string Name { get; private set; }  // Encapsulare
    
    public void SetName(string name)          // Validare în setter
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name cannot be empty.");
        Name = name;
    }
}
```

### 2. Moștenire

```
EntityBase (abstract)
    ├── Customer
    ├── Order
    ├── Courier (abstract)
    │       ├── BikeCourier
    │       └── CarCourier
    └── Delivery
```

### 3. Polimorfism

```csharp
// Courier.cs - metode abstracte
public abstract class Courier : EntityBase
{
    public abstract decimal MaxWeight { get; }
    public abstract TimeSpan CalculateDeliveryTime(decimal distanceKm);
}

// BikeCourier.cs - implementare specifică
public class BikeCourier : Courier
{
    public override decimal MaxWeight => 5.0m;
    public override TimeSpan CalculateDeliveryTime(decimal distanceKm)
    {
        return TimeSpan.FromMinutes((double)(distanceKm * 3.0m));  // 3 min/km
    }
}

// CarCourier.cs - implementare specifică
public class CarCourier : Courier
{
    public override decimal MaxWeight => 50.0m;
    public override TimeSpan CalculateDeliveryTime(decimal distanceKm)
    {
        return TimeSpan.FromMinutes((double)(distanceKm * 1.5m));  // 1.5 min/km
    }
}
```

## Principii SOLID Demonstrate - Exemple Concrete

### SRP (Single Responsibility Principle)

**Principiu**: Fiecare clasă trebuie să aibă o singură responsabilitate și un singur motiv pentru a se schimba.

#### Exemplu 1: OrderService - DOAR gestionare comenzi

```7:134:DeliverySystem.Services/OrderService.cs
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INotificationService _notificationService;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public Order CreateOrder(Guid customerId, IEnumerable<OrderItem> items)
    {
        var customer = _customerRepository.GetById(customerId);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {customerId} not found.");

        var order = new Order(customerId);

        foreach (var item in items)
        {
            order.AddItem(item);
        }

        _orderRepository.Add(order);
        _notificationService.NotifyOrderCreated(order, customer);

        return order;
    }

    public void ConfirmOrder(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        var customer = GetCustomerOrThrow(order.CustomerId);

        order.UpdateStatus(OrderStatus.Confirmed);
        _orderRepository.Update(order);
        _notificationService.NotifyOrderStatusChanged(order, customer);
    }
    // ... alte metode pentru gestionare comenzi
}
```

**De ce respectă SRP?**
- `OrderService` are o singură responsabilitate: gestionarea comenzilor
- Nu gestionează livrările (asta face `DeliveryService`)
- Nu persistă date direct (folosește `IOrderRepository`)
- Nu trimite notificări direct (folosește `INotificationService`)

#### Exemplu 2: DeliveryService - DOAR gestionare livrări

```7:66:DeliverySystem.Services/DeliveryService.cs
public class DeliveryService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICourierRepository _courierRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INotificationService _notificationService;

    public Delivery AssignCourierToOrder(Guid orderId, Guid courierId, decimal distanceKm)
    {
        var order = _orderRepository.GetById(orderId);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found.");

        if (order.Status != OrderStatus.ReadyForDelivery)
            throw new InvalidOperationException($"Order must be in ReadyForDelivery status. Current: {order.Status}");

        var courier = _courierRepository.GetById(courierId);
        if (courier == null)
            throw new InvalidOperationException($"Courier with ID {courierId} not found.");

        if (!courier.IsAvailable)
            throw new InvalidOperationException($"Courier {courier.Name} is not available.");

        var orderWeight = order.GetTotalWeight();
        if (!courier.CanCarry(orderWeight))
            throw new InvalidOperationException(
                $"Courier cannot carry this order. Order weight: {orderWeight}kg, Max capacity: {courier.MaxWeight}kg");

        var customer = _customerRepository.GetById(order.CustomerId);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {order.CustomerId} not found.");

        var delivery = new Delivery(orderId, courierId, distanceKm);
        var estimatedTime = courier.CalculateDeliveryTime(distanceKm);
        delivery.SetEstimatedDeliveryTime(estimatedTime);
        delivery.MarkAsAssigned();

        courier.SetUnavailable();
        _courierRepository.Update(courier);

        _deliveryRepository.Add(delivery);
        _notificationService.NotifyDeliveryAssigned(delivery, customer, courier);

        return delivery;
    }
}
```

**De ce respectă SRP?**
- `DeliveryService` gestionează DOAR livrările
- Nu creează comenzi (asta face `OrderService`)
- Fiecare serviciu are o responsabilitate clară și separată

---

### OCP (Open/Closed Principle)

**Principiu**: Clasele trebuie să fie deschise pentru extensie, dar închise pentru modificare.

#### Exemplu: Courier Hierarchy - Extensibil fără modificare

```5:64:DeliverySystem.Domain/Entities/Courier.cs
public abstract class Courier : EntityBase
{
    public string Name { get; private set; }
    public string Phone { get; private set; }
    public bool IsAvailable { get; private set; }

    public abstract VehicleType VehicleType { get; }
    public abstract decimal MaxWeight { get; }
    public abstract TimeSpan CalculateDeliveryTime(decimal distanceKm);

    protected Courier(string name, string phone)
    {
        SetName(name);
        SetPhone(phone);
        IsAvailable = true;
    }

    public bool CanCarry(decimal weight)
    {
        return weight <= MaxWeight;
    }
}
```

**Implementare BikeCourier** (fără modificarea clasei Courier):

```5:34:DeliverySystem.Domain/Entities/BikeCourier.cs
public class BikeCourier : Courier
{
    private const decimal BikeMaxWeightKg = 5.0m;
    private const decimal MinutesPerKm = 3.0m;

    public override VehicleType VehicleType => VehicleType.Bicycle;
    public override decimal MaxWeight => BikeMaxWeightKg;

    public BikeCourier(string name, string phone) : base(name, phone)
    {
    }

    public override TimeSpan CalculateDeliveryTime(decimal distanceKm)
    {
        if (distanceKm < 0)
            throw new ArgumentException("Distance cannot be negative.", nameof(distanceKm));

        var minutes = (double)(distanceKm * MinutesPerKm);
        return TimeSpan.FromMinutes(minutes);
    }
}
```

**Implementare CarCourier** (fără modificarea clasei Courier):

```5:46:DeliverySystem.Domain/Entities/CarCourier.cs
public class CarCourier : Courier
{
    private const decimal CarMaxWeightKg = 50.0m;
    private const decimal MinutesPerKm = 1.5m;

    public string LicensePlate { get; private set; }

    public override VehicleType VehicleType => VehicleType.Car;
    public override decimal MaxWeight => CarMaxWeightKg;

    public CarCourier(string name, string phone, string licensePlate) : base(name, phone)
    {
        SetLicensePlate(licensePlate);
    }

    public override TimeSpan CalculateDeliveryTime(decimal distanceKm)
    {
        if (distanceKm < 0)
            throw new ArgumentException("Distance cannot be negative.", nameof(distanceKm));

        var minutes = (double)(distanceKm * MinutesPerKm);
        return TimeSpan.FromMinutes(minutes);
    }
}
```

**De ce respectă OCP?**
- ✅ Poți adăuga `TruckCourier` fără să modifici `Courier`, `BikeCourier` sau `CarCourier`
- ✅ Codul existent rămâne neschimbat
- ✅ Extensibilitate prin moștenire

**Exemplu de extensie viitoare (fără modificare):**
```csharp
// Poți adăuga fără să modifici codul existent:
public class TruckCourier : Courier
{
    public override VehicleType VehicleType => VehicleType.Truck;
    public override decimal MaxWeight => 500.0m;
    
    public override TimeSpan CalculateDeliveryTime(decimal distanceKm)
    {
        return TimeSpan.FromMinutes((double)(distanceKm * 2.0m));
    }
}
```

---

### LSP (Liskov Substitution Principle)

**Principiu**: Obiectele unei superclase trebuie să poată fi înlocuite cu obiecte ale subclaselor fără a afecta funcționalitatea.

#### Exemplu: BikeCourier și CarCourier substituie Courier

**Clasa de bază Courier**:
```55:58:DeliverySystem.Domain/Entities/Courier.cs
    public bool CanCarry(decimal weight)
    {
        return weight <= MaxWeight;
    }
```

**Utilizare polimorfică în DeliveryService**:

```38:56:DeliverySystem.Services/DeliveryService.cs
        var courier = _courierRepository.GetById(courierId);
        if (courier == null)
            throw new InvalidOperationException($"Courier with ID {courierId} not found.");

        if (!courier.IsAvailable)
            throw new InvalidOperationException($"Courier {courier.Name} is not available.");

        var orderWeight = order.GetTotalWeight();
        if (!courier.CanCarry(orderWeight))
            throw new InvalidOperationException(
                $"Courier cannot carry this order. Order weight: {orderWeight}kg, Max capacity: {courier.MaxWeight}kg");

        var customer = _customerRepository.GetById(order.CustomerId);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {order.CustomerId} not found.");

        var delivery = new Delivery(orderId, courierId, distanceKm);
        var estimatedTime = courier.CalculateDeliveryTime(distanceKm);
```

**Repository returnează Courier (poate fi BikeCourier sau CarCourier)**:

```5:14:DeliverySystem.Interfaces/ICourierRepository.cs
public interface ICourierRepository
{
    Courier? GetById(Guid id);
    IEnumerable<Courier> GetAll();
    IEnumerable<Courier> GetAvailable();
    IEnumerable<Courier> GetAvailableForWeight(decimal weight);
    void Add(Courier courier);
    void Update(Courier courier);
    void Delete(Guid id);
}
```

**De ce respectă LSP?**
- ✅ `BikeCourier` și `CarCourier` pot înlocui `Courier` peste tot
- ✅ Metoda `CanCarry()` funcționează corect pentru ambele tipuri
- ✅ Metoda `CalculateDeliveryTime()` este apelată polimorfic
- ✅ Comportamentul respectă contractul clasei de bază

**Exemplu de utilizare în cod**:
```csharp
// Funcționează cu orice tip de Courier
Courier courier1 = new BikeCourier("Vasile", "+373-69-111111");
Courier courier2 = new CarCourier("Mihai", "+373-69-222222", "ABC-123");

// Ambele pot fi folosite identic
var time1 = courier1.CalculateDeliveryTime(10);  // 30 minute
var time2 = courier2.CalculateDeliveryTime(10);   // 15 minute

bool canCarry1 = courier1.CanCarry(3.0m);  // true (5kg max)
bool canCarry2 = courier2.CanCarry(3.0m);  // true (50kg max)
```

---

### ISP (Interface Segregation Principle)

**Principiu**: Clasele nu trebuie să fie forțate să implementeze interfețe pe care nu le folosesc.

#### Exemplu: Interfețe mici și focusate

**IOrderRepository - DOAR operații pe Order**:

```5:13:DeliverySystem.Interfaces/IOrderRepository.cs
public interface IOrderRepository
{
    Order? GetById(Guid id);
    IEnumerable<Order> GetAll();
    IEnumerable<Order> GetByCustomerId(Guid customerId);
    void Add(Order order);
    void Update(Order order);
    void Delete(Guid id);
}
```

**ICourierRepository - DOAR operații pe Courier**:

```5:14:DeliverySystem.Interfaces/ICourierRepository.cs
public interface ICourierRepository
{
    Courier? GetById(Guid id);
    IEnumerable<Courier> GetAll();
    IEnumerable<Courier> GetAvailable();
    IEnumerable<Courier> GetAvailableForWeight(decimal weight);
    void Add(Courier courier);
    void Update(Courier courier);
    void Delete(Guid id);
}
```

**Implementare separată**:

```6:55:DeliverySystem.Infrastructure/Repositories/InMemoryOrderRepository.cs
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = new();

    public Order? GetById(Guid id)
    {
        _orders.TryGetValue(id, out var order);
        return order;
    }

    public IEnumerable<Order> GetAll()
    {
        return _orders.Values.ToList();
    }

    public IEnumerable<Order> GetByCustomerId(Guid customerId)
    {
        return _orders.Values.Where(o => o.CustomerId == customerId).ToList();
    }

    public void Add(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (_orders.ContainsKey(order.Id))
            throw new InvalidOperationException($"Order with ID {order.Id} already exists.");

        _orders[order.Id] = order;
    }

    public void Update(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (!_orders.ContainsKey(order.Id))
            throw new InvalidOperationException($"Order with ID {order.Id} not found.");

        _orders[order.Id] = order;
    }

    public void Delete(Guid id)
    {
        if (!_orders.ContainsKey(id))
            throw new InvalidOperationException($"Order with ID {id} not found.");

        _orders.Remove(id);
    }
}
```

**De ce respectă ISP?**
- ✅ `IOrderRepository` conține DOAR metode pentru Order
- ✅ `ICourierRepository` conține DOAR metode pentru Courier
- ✅ Nu există metode inutile pe care repository-urile să fie forțate să le implementeze
- ✅ Fiecare repository implementează doar ce îi trebuie

**❌ Ce NU am făcut (violare ISP):**
```csharp
// BAD: Interfață mare cu toate metodele
public interface IRepository
{
    // Order methods
    Order? GetOrderById(Guid id);
    void AddOrder(Order order);
    
    // Courier methods
    Courier? GetCourierById(Guid id);
    void AddCourier(Courier courier);
    
    // Delivery methods
    Delivery? GetDeliveryById(Guid id);
    // ... etc
}
// Acest lucru ar forța clasele să implementeze metode pe care nu le folosesc!
```

---

### DIP (Dependency Inversion Principle)

**Principiu**: Modulele de nivel înalt nu trebuie să depindă de modulele de nivel scăzut. Ambele trebuie să depindă de abstracții.

#### Exemplu: Services depind de Interfețe, nu Implementări

**OrderService - Constructor Injection cu Interfețe**:

```7:21:DeliverySystem.Services/OrderService.cs
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INotificationService _notificationService;

    public OrderService(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }
```

**DeliveryService - Constructor Injection cu Interfețe**:

```7:27:DeliverySystem.Services/DeliveryService.cs
public class DeliveryService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICourierRepository _courierRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INotificationService _notificationService;

    public DeliveryService(
        IDeliveryRepository deliveryRepository,
        IOrderRepository orderRepository,
        ICourierRepository courierRepository,
        ICustomerRepository customerRepository,
        INotificationService notificationService)
    {
        _deliveryRepository = deliveryRepository ?? throw new ArgumentNullException(nameof(deliveryRepository));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }
```

**Wiring în Program.cs - Dependency Injection**:

```34:56:DeliverySystem.Console/Program.cs
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
```

**De ce respectă DIP?**
- ✅ `OrderService` depinde de `IOrderRepository` (interfață), NU de `InMemoryOrderRepository` (implementare)
- ✅ Poți schimba implementarea (ex: `SqlOrderRepository`) fără să modifici `OrderService`
- ✅ Dependențele sunt injectate prin constructor
- ✅ High-level modules (Services) depind de abstractions (Interfaces)
- ✅ Low-level modules (Infrastructure) implementează abstractions

**Beneficii:**
```csharp
// Poți schimba implementarea fără să modifici OrderService:
// În loc de:
IOrderRepository orderRepository = new InMemoryOrderRepository();

// Poți folosi:
IOrderRepository orderRepository = new SqlOrderRepository(connectionString);
// OrderService funcționează la fel!
```

---

## Rezumat Principii SOLID

| Principiu | Exemplu din Cod | Beneficiu |
|-----------|----------------|-----------|
| **SRP** | `OrderService` vs `DeliveryService` | Fiecare clasă are o responsabilitate clară |
| **OCP** | `Courier` abstract → `BikeCourier`, `CarCourier` | Extensibil fără modificare |
| **LSP** | `BikeCourier`/`CarCourier` substituie `Courier` | Polimorfism corect |
| **ISP** | `IOrderRepository` vs `ICourierRepository` | Interfețe mici, focused |
| **DIP** | Services depind de Interfaces | Testabil, flexibil, ușor de schimbat |

## Relații între Entități

| Relație | Tip | Descriere |
|---------|-----|-----------|
| Customer → Address | Compoziție | Customer conține Address |
| Order → OrderItem | Compoziție | Order conține lista de OrderItem |
| Order → Customer | Asociere | Order aparține unui Customer |
| Delivery → Order | Asociere | Delivery este pentru un Order |
| Delivery → Courier | Asociere | Delivery este asignat unui Courier |
| BikeCourier → Courier | Moștenire | BikeCourier extinde Courier |
| CarCourier → Courier | Moștenire | CarCourier extinde Courier |

## Fluxul Aplicației

1. **Creare Customer** cu Address
2. **Creare Order** cu OrderItems
3. **Confirmare și Procesare** Order
4. **Selectare Courier** disponibil (polimorfism)
5. **Creare Delivery** și asignare Courier
6. **Actualizare Status** Delivery (PickedUp → InTransit → Delivered)
7. **Notificări** către Customer
