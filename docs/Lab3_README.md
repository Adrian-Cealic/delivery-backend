# Laborator 3 — Builder, Prototype & Singleton

## Obiectiv

Implementarea a trei paternuri de proiectare creaționale în cadrul Delivery Management System:
- **Builder** — construcția pas cu pas a obiectelor complexe
- **Prototype** — clonarea obiectelor existente (shallow & deep copy)
- **Singleton** — instanță unică, globală, thread-safe

---

## 1. Builder Pattern

### Problemă rezolvată
Crearea unui `Order` necesită mai mulți pași: setarea clientului, adăugarea produselor, alegerea priorității și notelor de livrare. Builder-ul separă logica de construcție de reprezentarea finală.

### UML

```
@startuml
interface IOrderBuilder {
  +SetCustomerId(id): IOrderBuilder
  +AddItem(item): IOrderBuilder
  +SetPriority(p): IOrderBuilder
  +SetDeliveryNotes(n): IOrderBuilder
  +Build(): Order
  +Reset(): IOrderBuilder
}

class StandardOrderBuilder {
  -_customerId: Guid
  -_items: List<OrderItem>
  -_priority: OrderPriority
  -_notes: string?
  +SetCustomerId(id): IOrderBuilder
  +AddItem(item): IOrderBuilder
  +SetPriority(p): IOrderBuilder
  +SetDeliveryNotes(n): IOrderBuilder
  +Build(): Order
  +Reset(): IOrderBuilder
}

class OrderDirector {
  -_builder: IOrderBuilder
  +BuildStandardOrder(custId, items): Order
  +BuildExpressOrder(custId, items): Order
  +BuildEconomyOrder(custId, items): Order
}

class Order {
  +Id: Guid
  +CustomerId: Guid
  +Items: List<OrderItem>
  +Priority: OrderPriority
  +DeliveryNotes: string?
}

OrderDirector o--> IOrderBuilder
StandardOrderBuilder ..|> IOrderBuilder
StandardOrderBuilder ..> Order : creates
@enduml
```

### Implementare

**IOrderBuilder** — interfață cu API fluent:

```csharp
public interface IOrderBuilder
{
    IOrderBuilder SetCustomerId(Guid customerId);
    IOrderBuilder AddItem(OrderItem item);
    IOrderBuilder SetPriority(OrderPriority priority);
    IOrderBuilder SetDeliveryNotes(string? notes);
    IOrderBuilder Reset();
    Order Build();
}
```

**StandardOrderBuilder** — implementare concretă:

```csharp
var builder = new StandardOrderBuilder();
var order = builder
    .SetCustomerId(customerId)
    .AddItem(new OrderItem("Laptop", 1, 999.99m, 3.0m))
    .SetPriority(OrderPriority.Express)
    .SetDeliveryNotes("Handle with care")
    .Build();
```

**OrderDirector** — preseturi reutilizabile:

```csharp
var director = new OrderDirector(new StandardOrderBuilder());
var expressOrder = director.BuildExpressOrder(customerId, items, "Urgent");
var economyOrder = director.BuildEconomyOrder(customerId, items);
```

### Principii SOLID aplicate
- **SRP**: Builder-ul construiește, Director-ul orchestrează, Order-ul este entitate de domeniu
- **OCP**: Noi tipuri de builder-e (ex. `BulkOrderBuilder`) se adaugă fără a modifica codul existent
- **DIP**: Director-ul depinde de `IOrderBuilder`, nu de `StandardOrderBuilder`

### API Endpoint
- `POST /api/orders/builder` — creează o comandă folosind Builder pattern

---

## 2. Prototype Pattern

### Problemă rezolvată
Permite duplicarea rapidă a unei comenzi existente (scenariul re-order), fără a reconstrui obiectul de la zero.

### UML

```
@startuml
interface "IPrototype<T>" as IPrototype {
  +Clone(): T
  +DeepCopy(): T
}

class Order {
  +Id: Guid
  +CustomerId: Guid
  -_items: List<OrderItem>
  +Priority: OrderPriority
  +Clone(): Order
  +DeepCopy(): Order
}

class OrderItem {
  +ProductName: string
  +Quantity: int
  +UnitPrice: decimal
  +Weight: decimal
}

Order ..|> IPrototype
Order *-- OrderItem : deep copies
Order ..> Order : "Clone() / DeepCopy()"
@enduml
```

### Implementare

**IPrototype<T>** — interfață generică:

```csharp
public interface IPrototype<T>
{
    T Clone();
    T DeepCopy();
}
```

**Order.Clone()** — shallow copy (partajează referințele la OrderItem):

```csharp
public Order Clone()
{
    var clone = new Order(CustomerId);
    clone.Priority = Priority;
    clone.DeliveryNotes = DeliveryNotes;
    foreach (var item in _items)
        clone.AddItem(item);  // aceeași referință
    return clone;
}
```

**Order.DeepCopy()** — deep copy (creează instanțe noi de OrderItem):

```csharp
public Order DeepCopy()
{
    var clone = new Order(CustomerId);
    clone.Priority = Priority;
    clone.DeliveryNotes = DeliveryNotes;
    foreach (var item in _items)
        clone.AddItem(new OrderItem(item.ProductName, item.Quantity,
            item.UnitPrice, item.Weight));  // instanță nouă
    return clone;
}
```

### Shallow vs Deep Copy

| Aspect | Clone() | DeepCopy() |
|--------|---------|------------|
| Order ID | Nou | Nou |
| CustomerId | Copiat | Copiat |
| Items list | Listă nouă, referințe comune | Listă nouă, instanțe noi |
| Status | Reset la Created | Reset la Created |
| Performanță | Mai rapidă | Mai lentă (copiere completă) |

### Principii SOLID aplicate
- **SRP**: Logica de clonare este în entitate, nu în serviciu
- **OCP**: Noi entități pot implementa `IPrototype<T>` fără modificări
- **LSP**: `Order` ca `IPrototype<Order>` respectă contractul complet

### API Endpoint
- `POST /api/orders/{id}/clone` — clonează o comandă existentă

---

## 3. Singleton Pattern

### Problemă rezolvată
Configurația sistemului (distanță maximă, monedă, limite) trebuie să fie unică și accesibilă global, thread-safe.

### UML

```
@startuml
class DeliverySystemConfiguration {
  -{static} _instance: Lazy<DeliverySystemConfiguration>
  -_lock: object
  -DeliverySystemConfiguration()
  +{static} Instance: DeliverySystemConfiguration
  +MaxDeliveryDistanceKm: decimal
  +DefaultCurrency: string
  +MaxOrderItems: int
  +SystemName: string
  +SetMaxDeliveryDistance(d): void
  +SetDefaultCurrency(c): void
  +SetMaxOrderItems(n): void
  +SetSystemName(n): void
}

note right of DeliverySystemConfiguration
  Thread-safe via Lazy<T>
  Private constructor prevents
  external instantiation
  _lock for thread-safe updates
end note

class DeliveryService {
  +AssignCourierToOrder()
}

DeliveryService ..> DeliverySystemConfiguration : uses via DI
@enduml
```

### Implementare

```csharp
public sealed class DeliverySystemConfiguration
{
    private static readonly Lazy<DeliverySystemConfiguration> _instance =
        new(() => new DeliverySystemConfiguration());

    private readonly object _lock = new();

    public static DeliverySystemConfiguration Instance => _instance.Value;

    public decimal MaxDeliveryDistanceKm { get; private set; }
    public string DefaultCurrency { get; private set; }
    public int MaxOrderItems { get; private set; }
    public string SystemName { get; private set; }

    private DeliverySystemConfiguration()
    {
        MaxDeliveryDistanceKm = 100.0m;
        DefaultCurrency = "MDL";
        MaxOrderItems = 50;
        SystemName = "Delivery Management System";
    }

    public void SetMaxDeliveryDistance(decimal distance)
    {
        if (distance <= 0)
            throw new ArgumentException("Must be positive.");
        lock (_lock) { MaxDeliveryDistanceKm = distance; }
    }
}
```

### Integrare cu DI

Singleton-ul este înregistrat în containerul de DI și injectat în servicii:

```csharp
// Program.cs
builder.Services.AddSingleton(DeliverySystemConfiguration.Instance);

// DeliveryService.cs — inject via constructor
public DeliveryService(..., DeliverySystemConfiguration config)
{
    _config = config;
}
```

### Thread Safety
- `Lazy<T>` garantează inițializarea thread-safe
- `lock` protejează operațiile de scriere
- Proprietățile cu `private set` previn modificări necontrolate

### Principii SOLID aplicate
- **SRP**: Configurația are o singură responsabilitate — setări de sistem
- **DIP**: Serviciile primesc configurația prin DI, nu acces static direct
- **OCP**: Noi setări se adaugă fără a modifica consumatorii existenți

### API Endpoint
- `GET /api/config` — returnează configurația curentă

---

## Teste Unitare

### OrderBuilderTests (11 teste)
- Build cu toate câmpurile
- Build cu mai multe items
- Prioritate default Normal
- Excepții fără customerId / items
- Reset curăță toate câmpurile
- Fluent API returnează aceeași instanță
- Director: standard, express, economy orders
- Director: ID-uri diferite la ordere multiple

### OrderPrototypeTests (12 teste)
- Clone creează ID diferit
- Clone păstrează CustomerId, Priority, DeliveryNotes
- Clone: shallow — aceleași referințe la items
- DeepCopy: ID diferit, instanțe noi de items
- DeepCopy: valori egale dar referințe diferite
- Independență: modificarea clonei nu afectează originalul
- Prețul total se păstrează la clonare

### SingletonTests (9 teste)
- Instance returnează aceeași referință
- Valori default corecte
- Setters actualizează valorile
- Excepții la valori invalide
- Thread safety: 10 accesări concurente
- Persistența setărilor între referințe

**Total: 53 teste (20 Lab2 + 33 Lab3) — toate Passed**

---

## Structura Fișierelor Lab 3

```
DeliverySystem.Domain/
├── Builders/
│   ├── IOrderBuilder.cs          ← Builder interface
│   ├── StandardOrderBuilder.cs   ← Concrete builder
│   └── OrderDirector.cs          ← Director with presets
├── Prototypes/
│   └── IPrototype.cs             ← Prototype interface
├── Entities/
│   └── Order.cs                  ← Updated: Priority, Notes, Clone, DeepCopy
└── Enums/
    └── OrderPriority.cs          ← New enum

DeliverySystem.Infrastructure/
└── Configuration/
    └── DeliverySystemConfiguration.cs  ← Singleton

DeliverySystem.Tests/
└── Lab3/
    ├── OrderBuilderTests.cs
    ├── OrderPrototypeTests.cs
    └── SingletonTests.cs
```
