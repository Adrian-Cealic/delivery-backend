# Laborator 2 — Paternuri Creaționale: Factory Method și Abstract Factory

## Cuprins
1. [Factory Method Pattern](#1-factory-method-pattern)
2. [Abstract Factory Pattern](#2-abstract-factory-pattern)
3. [Testare](#3-testare)
4. [Beneficii și Concluzii](#4-beneficii-și-concluzii)

---

## 1. Factory Method Pattern

### Definiție
Factory Method definește o interfață pentru crearea unui obiect, dar permite subclaselor să decidă ce clasă concretă va fi instanțiată. Crearea obiectului este delegată unei metode abstracte pe care fiecare fabrică concretă o implementează.

### Diagrama UML

```
          ┌─────────────────────────────┐
          │     CourierFactory           │  ← Creator (abstract)
          │─────────────────────────────│
          │ + CreateCourier(params)     │  ← Factory Method (abstract)
          │ + CreateAndValidate(params) │  ← Template Method
          └──────────┬──────────────────┘
                     │
       ┌─────────────┼────────────────┐
       │             │                │
┌──────▼──────┐ ┌───▼──────────┐ ┌───▼──────────────┐
│BikeCourier  │ │CarCourier    │ │DroneCourier      │
│Factory      │ │Factory       │ │Factory           │
│─────────────│ │──────────────│ │──────────────────│
│+CreateCourier│ │+CreateCourier│ │+CreateCourier    │
│→ BikeCourier│ │→ CarCourier  │ │→ DroneCourier    │
└─────────────┘ └──────────────┘ └──────────────────┘
```

### Aplicare în proiect — Crearea Curierilor

**Problema:** Controller-ul API crea direct obiecte `new BikeCourier(...)` și `new CarCourier(...)`. Adăugarea unui nou tip de curier necesita modificarea controller-ului, încălcând principiul Open/Closed (OCP).

**Soluția:** Clasa abstractă `CourierFactory` cu metoda fabrică `CreateCourier()`, implementată de fiecare fabrică concretă.

#### Clasa abstractă — CourierFactory
```csharp
// DeliverySystem.Domain/Factories/CourierFactory.cs
public abstract class CourierFactory
{
    public abstract Courier CreateCourier(CourierCreationParams parameters);

    public Courier CreateAndValidate(CourierCreationParams parameters)
    {
        if (parameters == null)
            throw new ArgumentNullException(nameof(parameters));
        if (string.IsNullOrWhiteSpace(parameters.Name))
            throw new ArgumentException("Courier name is required.");
        if (string.IsNullOrWhiteSpace(parameters.Phone))
            throw new ArgumentException("Courier phone is required.");

        var courier = CreateCourier(parameters);
        return courier;
    }
}
```

#### Fabrici concrete
```csharp
// DeliverySystem.Domain/Factories/BikeCourierFactory.cs
public class BikeCourierFactory : CourierFactory
{
    public override Courier CreateCourier(CourierCreationParams parameters)
    {
        return new BikeCourier(parameters.Name, parameters.Phone);
    }
}

// DeliverySystem.Domain/Factories/CarCourierFactory.cs
public class CarCourierFactory : CourierFactory
{
    public override Courier CreateCourier(CourierCreationParams parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.LicensePlate))
            throw new ArgumentException("License plate is required for car couriers.");
        return new CarCourier(parameters.Name, parameters.Phone, parameters.LicensePlate);
    }
}

// DeliverySystem.Domain/Factories/DroneCourierFactory.cs
public class DroneCourierFactory : CourierFactory
{
    public override Courier CreateCourier(CourierCreationParams parameters)
    {
        if (!parameters.MaxFlightRangeKm.HasValue || parameters.MaxFlightRangeKm <= 0)
            throw new ArgumentException("Max flight range is required for drone couriers.");
        return new DroneCourier(parameters.Name, parameters.Phone, parameters.MaxFlightRangeKm.Value);
    }
}
```

#### Utilizare în Controller
```csharp
// DeliverySystem.API/Controllers/CouriersController.cs — POST /api/couriers
[HttpPost]
public ActionResult<CourierResponse> CreateCourier([FromBody] CreateCourierRequest request)
{
    if (!Enum.TryParse<VehicleType>(request.VehicleType, true, out var vehicleType))
        return BadRequest(new { message = "Invalid vehicle type" });

    var factory = CourierFactoryProvider.GetFactory(vehicleType);
    var parameters = new CourierCreationParams(
        request.Name, request.Phone, request.LicensePlate, request.MaxFlightRangeKm);

    var courier = factory.CreateAndValidate(parameters);
    _courierRepository.Add(courier);

    return CreatedAtAction(nameof(GetById), new { id = courier.Id }, MapToResponse(courier));
}
```

#### CourierFactoryProvider — Rezolvare fabrică după tip
```csharp
// DeliverySystem.Domain/Factories/CourierFactoryProvider.cs
public static class CourierFactoryProvider
{
    private static readonly Dictionary<VehicleType, CourierFactory> Factories = new()
    {
        { VehicleType.Bicycle, new BikeCourierFactory() },
        { VehicleType.Car, new CarCourierFactory() },
        { VehicleType.Drone, new DroneCourierFactory() }
    };

    public static CourierFactory GetFactory(VehicleType vehicleType)
    {
        if (!Factories.TryGetValue(vehicleType, out var factory))
            throw new ArgumentException($"No factory registered for vehicle type: {vehicleType}");
        return factory;
    }
}
```

---

## 2. Abstract Factory Pattern

### Definiție
Abstract Factory furnizează o interfață pentru crearea de familii de obiecte înrudite, fără a specifica clasele concrete. Garantează că obiectele create într-o familie sunt compatibile între ele.

### Diagrama UML

```
┌─────────────────────────────┐
│   INotificationFactory      │  ← Abstract Factory
│─────────────────────────────│
│ + CreateSender()            │ → INotificationSender
│ + CreateFormatter()         │ → INotificationFormatter
└──────────┬──────────────────┘
           │
     ┌─────┴──────────────┐
     │                    │
┌────▼────────────┐  ┌───▼─────────────────┐
│ConsoleNotif.    │  │EmailNotification    │
│Factory          │  │Factory              │
│─────────────────│  │─────────────────────│
│CreateSender()   │  │CreateSender()       │
│→ ConsoleSender  │  │→ EmailSender        │
│CreateFormatter()│  │CreateFormatter()    │
│→ PlainText      │  │→ Html               │
└─────────────────┘  └─────────────────────┘
```

### Aplicare în proiect — Sistemul de Notificări

**Problema:** `ConsoleNotificationService` era o clasă monolitică. Adăugarea unui canal nou (Email, SMS) necesita duplicarea întregii logici de formatare și trimitere.

**Soluția:** `INotificationFactory` creează familii de obiecte: un `INotificationSender` (trimite notificarea) și un `INotificationFormatter` (formatează mesajul). Fiecare canal (Console, Email) are propria fabrică ce produce perechi compatibile.

#### Interfețele Abstract Factory
```csharp
// DeliverySystem.Interfaces/Notifications/INotificationFactory.cs
public interface INotificationFactory
{
    INotificationSender CreateSender();
    INotificationFormatter CreateFormatter();
}

// DeliverySystem.Interfaces/Notifications/INotificationSender.cs
public interface INotificationSender
{
    void Send(string recipient, string subject, string body);
}

// DeliverySystem.Interfaces/Notifications/INotificationFormatter.cs
public interface INotificationFormatter
{
    string FormatOrderCreated(Order order, Customer customer);
    string FormatOrderStatusChanged(Order order, Customer customer);
    string FormatDeliveryAssigned(Delivery delivery, Customer customer, Courier courier);
    string FormatDeliveryStatusChanged(Delivery delivery, Customer customer);
    string FormatDeliveryCompleted(Delivery delivery, Customer customer);
}
```

#### Familia Console (PlainText + Console)
```csharp
// DeliverySystem.Infrastructure/Notifications/Factories/ConsoleNotificationFactory.cs
public class ConsoleNotificationFactory : INotificationFactory
{
    public INotificationSender CreateSender()
    {
        return new ConsoleNotificationSender();
    }

    public INotificationFormatter CreateFormatter()
    {
        return new PlainTextNotificationFormatter();
    }
}
```

#### Familia Email (HTML + Email)
```csharp
// DeliverySystem.Infrastructure/Notifications/Factories/EmailNotificationFactory.cs
public class EmailNotificationFactory : INotificationFactory
{
    public INotificationSender CreateSender()
    {
        return new EmailNotificationSender();
    }

    public INotificationFormatter CreateFormatter()
    {
        return new HtmlNotificationFormatter();
    }
}
```

#### Utilizare în NotificationService (consumatorul fabricii)
```csharp
// DeliverySystem.Infrastructure/Notifications/ConsoleNotificationService.cs
public class ConsoleNotificationService : INotificationService
{
    private readonly INotificationSender _sender;
    private readonly INotificationFormatter _formatter;

    public ConsoleNotificationService(INotificationFactory notificationFactory)
    {
        _sender = notificationFactory.CreateSender();
        _formatter = notificationFactory.CreateFormatter();
    }

    public void NotifyOrderCreated(Order order, Customer customer)
    {
        var body = _formatter.FormatOrderCreated(order, customer);
        _sender.Send(customer.Email, "Order Created", body);
    }
    // ... alte metode similare
}
```

#### Schimbarea canalului — doar o linie în DI
```csharp
// Program.cs — pentru Console:
builder.Services.AddSingleton<INotificationFactory, ConsoleNotificationFactory>();

// Program.cs — pentru Email (schimb fără a modifica alte clase):
// builder.Services.AddSingleton<INotificationFactory, EmailNotificationFactory>();
```

---

## 3. Testare

### Teste Factory Method (CourierFactoryTests.cs)
- `BikeCourierFactory_CreatesCourier_ReturnsBikeCourier` — verifică tipul corect
- `CarCourierFactory_CreatesCourier_ReturnsCarCourier` — verifică tipul + license plate
- `CarCourierFactory_WithoutLicensePlate_ThrowsArgumentException` — validare parametri
- `DroneCourierFactory_CreatesCourier_ReturnsDroneCourier` — verifică tipul + flight range
- `DroneCourierFactory_WithoutFlightRange_ThrowsArgumentException` — validare parametri
- `CourierFactoryProvider_ReturnsCorrectFactory_ForEachVehicleType` — provider corect
- `CreateAndValidate_WithNullParams_ThrowsArgumentNullException` — template method validare
- `CreateAndValidate_WithEmptyName_ThrowsArgumentException` — template method validare
- `DroneCourier_CalculateDeliveryTime_ExceedingRange_ThrowsException` — business rule
- `AllFactories_ProduceCouriers_ThatAreAvailableByDefault` — stare inițială corectă

### Teste Abstract Factory (NotificationFactoryTests.cs)
- `ConsoleFactory_CreatesSender_ReturnsConsoleNotificationSender` — tipul corect
- `ConsoleFactory_CreatesFormatter_ReturnsPlainTextFormatter` — perechea corectă
- `EmailFactory_CreatesSender_ReturnsEmailNotificationSender` — tipul corect
- `EmailFactory_CreatesFormatter_ReturnsHtmlFormatter` — perechea corectă
- `ConsoleFactory_FormatterOutput_DoesNotContainHtml` — format corect
- `EmailFactory_FormatterOutput_ContainsHtml` — format corect
- `BothFactories_ProduceWorkingPairs` — familii funcționale
- `PlainTextFormatter_FormatDeliveryAssigned_ContainsCourierInfo` — conținut
- `HtmlFormatter_FormatDeliveryCompleted_ContainsThankYou` — conținut

**Rezultat: 20/20 teste passed**

---

## 4. Beneficii și Concluzii

### Factory Method
| Beneficiu | Explicație |
|-----------|-----------|
| **OCP** | Adăugarea `DroneCourier` nu a necesitat modificarea fabricilor existente |
| **DIP** | Controller-ul depinde de `CourierFactory` abstract, nu de clase concrete |
| **SRP** | Fiecare fabrică gestionează crearea unui singur tip de curier |
| **Extensibilitate** | Pentru un nou tip (ex: `TruckCourier`) se adaugă doar o fabrică nouă |

### Abstract Factory
| Beneficiu | Explicație |
|-----------|-----------|
| **Consistență** | Console factory produce întotdeauna sender Console + formatter PlainText |
| **Decuplare** | `NotificationService` nu cunoaște implementările concrete |
| **Schimbabilitate** | Trecerea de la Console la Email = o linie în DI |
| **OCP** | Adăugarea unui canal SMS = o nouă fabrică, fără modificări |

### Fișiere noi/modificate

```
DeliverySystem.Domain/
  ├── Enums/VehicleType.cs              (modificat: + Drone)
  ├── Entities/DroneCourier.cs          (nou)
  └── Factories/
      ├── CourierCreationParams.cs       (nou)
      ├── CourierFactory.cs              (nou - abstract)
      ├── BikeCourierFactory.cs          (nou)
      ├── CarCourierFactory.cs           (nou)
      ├── DroneCourierFactory.cs         (nou)
      └── CourierFactoryProvider.cs      (nou)

DeliverySystem.Interfaces/
  └── Notifications/
      ├── INotificationFactory.cs        (nou)
      ├── INotificationSender.cs         (nou)
      └── INotificationFormatter.cs      (nou)

DeliverySystem.Infrastructure/
  └── Notifications/
      ├── ConsoleNotificationService.cs  (refactorizat)
      ├── Senders/
      │   ├── ConsoleNotificationSender.cs (nou)
      │   └── EmailNotificationSender.cs   (nou)
      ├── Formatters/
      │   ├── PlainTextNotificationFormatter.cs (nou)
      │   └── HtmlNotificationFormatter.cs      (nou)
      └── Factories/
          ├── ConsoleNotificationFactory.cs (nou)
          └── EmailNotificationFactory.cs   (nou)

DeliverySystem.Tests/
  ├── CourierFactoryTests.cs             (nou - 11 teste)
  └── NotificationFactoryTests.cs        (nou - 9 teste)
```
