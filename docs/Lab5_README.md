# Laborator 5 — Paternuri Structurale: Flyweight, Decorator, Bridge, Proxy

## Obiectiv

Implementarea a patru paternuri structurale în cadrul Delivery Management System:
- **Flyweight** — partajarea obiectelor pentru optimizarea memoriei (zone de livrare)
- **Decorator** — extinderea funcționalităților fără modificarea clasei de bază (notificări)
- **Bridge** — separarea abstractizării de implementare (generare rapoarte)
- **Proxy** — controlul accesului la resurse (protecție bazată pe roluri)

---

## 1. Flyweight Pattern

### Problemă rezolvată

Sistemul de livrare operează cu un număr finit de zone geografice (Center, Suburb, Rural etc.), dar fiecare livrare referă o zonă. Fără Flyweight, mii de livrări ar crea mii de obiecte identice DeliveryZone, consumând memorie inutil. Flyweight-ul garantează că fiecare zonă unică există într-o singură instanță partajată, indiferent de câte livrări o referă.

### Diagrama UML

```
@startuml
class DeliveryZone <<flyweight>> {
  +ZoneCode: string
  +ZoneName: string
  +BaseDeliveryFee: decimal
  +MaxWeightKg: decimal
  ~DeliveryZone(code, name, fee, maxWeight)
}

class DeliveryZoneFactory {
  -_cache: Dictionary<string, DeliveryZone>
  +GetZone(code, name, fee, maxWeight): DeliveryZone
  +CachedCount: int
  +GetAllCachedZones(): IReadOnlyCollection
}

DeliveryZoneFactory o--> DeliveryZone : creates / caches
@enduml
```

### Implementare

**DeliveryZone** — obiect flyweight immutabil cu stare intrinsecă:

```csharp
public sealed class DeliveryZone
{
    public string ZoneCode { get; }
    public string ZoneName { get; }
    public decimal BaseDeliveryFee { get; }
    public decimal MaxWeightKg { get; }

    internal DeliveryZone(string zoneCode, string zoneName, decimal baseDeliveryFee, decimal maxWeightKg)
    {
        ZoneCode = zoneCode;
        ZoneName = zoneName;
        BaseDeliveryFee = baseDeliveryFee;
        MaxWeightKg = maxWeightKg;
    }
}
```

**DeliveryZoneFactory** — factory cu cache intern, returnează aceeași instanță pentru același cod de zonă:

```csharp
public sealed class DeliveryZoneFactory
{
    private readonly Dictionary<string, DeliveryZone> _cache = new(StringComparer.OrdinalIgnoreCase);

    public DeliveryZone GetZone(string zoneCode, string zoneName, decimal baseDeliveryFee, decimal maxWeightKg)
    {
        if (_cache.TryGetValue(zoneCode, out var existing))
            return existing;

        var zone = new DeliveryZone(zoneCode, zoneName, baseDeliveryFee, maxWeightKg);
        _cache[zoneCode] = zone;
        return zone;
    }

    public int CachedCount => _cache.Count;
}
```

**Demonstrație:**
```csharp
var factory = new DeliveryZoneFactory();
var zone1 = factory.GetZone("CTR", "Center", 5.00m, 30m);
var zone2 = factory.GetZone("CTR", "Center", 5.00m, 30m);
// zone1 și zone2 referă exact aceeași instanță (ReferenceEquals == true)
// factory.CachedCount == 1
```

### Principii SOLID aplicate
- **SRP**: DeliveryZone conține doar starea intrinsecă; factory-ul gestionează doar cache-ul
- **OCP**: Noi zone se adaugă fără modificarea codului existent — factory le creează la cerere
- **DIP**: Constructorul intern al DeliveryZone forțează utilizarea factory-ului

---

## 2. Decorator Pattern

### Problemă rezolvată

Sistemul de notificări trebuie extins cu funcționalități noi (logging, SMS) fără a modifica `ConsoleNotificationService`. Decorator-ul permite compunerea dinamică a comportamentelor: fiecare decorator învăluie serviciul interior și adaugă propriul comportament înainte de a delega.

### Diagrama UML

```
@startuml
interface INotificationService {
  +NotifyOrderCreated(order, customer)
  +NotifyOrderStatusChanged(order, customer)
  +NotifyDeliveryAssigned(delivery, customer, courier)
  +NotifyDeliveryStatusChanged(delivery, customer)
  +NotifyDeliveryCompleted(delivery, customer)
}

class ConsoleNotificationService

abstract class NotificationServiceDecorator {
  #Inner: INotificationService
  +NotifyOrderCreated(order, customer)
  ...
}

class LoggingNotificationDecorator {
  -_logAction: Action<string>
  +NotifyOrderCreated(order, customer)
  ...
}

class SmsNotificationDecorator {
  -_smsAction: Action<string, string>
  +NotifyOrderCreated(order, customer)
  ...
}

INotificationService <|.. ConsoleNotificationService
INotificationService <|.. NotificationServiceDecorator
NotificationServiceDecorator <|-- LoggingNotificationDecorator
NotificationServiceDecorator <|-- SmsNotificationDecorator
NotificationServiceDecorator o--> INotificationService : _inner
@enduml
```

### Implementare

**NotificationServiceDecorator** — clasă abstractă de bază, delegă toate metodele la `Inner`:

```csharp
public abstract class NotificationServiceDecorator : INotificationService
{
    protected readonly INotificationService Inner;

    protected NotificationServiceDecorator(INotificationService inner)
    {
        Inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public virtual void NotifyOrderCreated(Order order, Customer customer)
        => Inner.NotifyOrderCreated(order, customer);
    // ... restul metodelor delegă la Inner
}
```

**LoggingNotificationDecorator** — loghează fiecare eveniment înainte de a delega:

```csharp
public sealed class LoggingNotificationDecorator : NotificationServiceDecorator
{
    public override void NotifyOrderCreated(Order order, Customer customer)
    {
        _logAction($"NotifyOrderCreated — Order {order.Id}, Customer {customer.Name}");
        base.NotifyOrderCreated(order, customer);
    }
}
```

**SmsNotificationDecorator** — trimite un SMS simulat înainte de a delega:

```csharp
public sealed class SmsNotificationDecorator : NotificationServiceDecorator
{
    public override void NotifyOrderCreated(Order order, Customer customer)
    {
        _smsAction(customer.Phone, $"Order {order.Id} created.");
        base.NotifyOrderCreated(order, customer);
    }
}
```

**Lanț DI (Program.cs):**
```csharp
builder.Services.AddSingleton<INotificationService>(sp =>
{
    var factory = sp.GetRequiredService<INotificationFactory>();
    INotificationService inner = new ConsoleNotificationService(factory);
    inner = new SmsNotificationDecorator(inner);
    return new LoggingNotificationDecorator(inner);
});
// Flux: LoggingDecorator → SmsDecorator → ConsoleNotificationService
```

### Principii SOLID aplicate
- **OCP**: Funcționalități noi (Push notifications) se adaugă ca decoratori noi, fără a modifica clasele existente
- **SRP**: Fiecare decorator are o singură responsabilitate (logging, SMS)
- **LSP**: Orice decorator este substituibil ca INotificationService
- **DIP**: Decoratorii depind de abstracția INotificationService, nu de implementări concrete

---

## 3. Bridge Pattern

### Problemă rezolvată

Sistemul trebuie să genereze rapoarte (Order Summary, Delivery Status) în formate diferite (text consolă, JSON). Fără Bridge, fiecare combinație (OrderSummary+Console, OrderSummary+Json, DeliveryStatus+Console, ...) ar necesita o clasă separată. Bridge-ul separă *ce raportăm* (abstractizare) de *cum renderizăm* (implementare), permițând extensibilitate independentă pe ambele axe.

### Diagrama UML

```
@startuml
abstract class DeliveryReport <<abstraction>> {
  #Renderer: IReportRenderer
  +Generate(): string
}

class OrderSummaryReport {
  -_orders: IEnumerable<Order>
  +Generate(): string
}

class DeliveryStatusReport {
  -_deliveries: IEnumerable<Delivery>
  +Generate(): string
}

interface IReportRenderer <<implementation>> {
  +Render(title, data): string
}

class ConsoleReportRenderer {
  +Render(title, data): string
}

class JsonReportRenderer {
  +Render(title, data): string
}

DeliveryReport <|-- OrderSummaryReport
DeliveryReport <|-- DeliveryStatusReport
DeliveryReport o--> IReportRenderer
IReportRenderer <|.. ConsoleReportRenderer
IReportRenderer <|.. JsonReportRenderer
@enduml
```

### Implementare

**DeliveryReport** — abstractizare cu referință la renderer:

```csharp
public abstract class DeliveryReport
{
    protected IReportRenderer Renderer { get; }

    protected DeliveryReport(IReportRenderer renderer)
    {
        Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
    }

    public abstract string Generate();
}
```

**OrderSummaryReport** — construiește dicționar de date, delegă la renderer:

```csharp
public sealed class OrderSummaryReport : DeliveryReport
{
    public override string Generate()
    {
        var data = new Dictionary<string, string>
        {
            ["Total Orders"] = orderList.Count.ToString(),
            ["Total Revenue"] = orderList.Sum(o => o.GetTotalPrice()).ToString("C"),
            // ...
        };
        return Renderer.Render("Order Summary Report", data);
    }
}
```

**ConsoleReportRenderer** — format text cu tabel:
```csharp
public sealed class ConsoleReportRenderer : IReportRenderer
{
    public string Render(string title, IReadOnlyDictionary<string, string> data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(new string('=', 50));
        sb.AppendLine($"  {title}");
        foreach (var kvp in data)
            sb.AppendLine($"  {kvp.Key,-25} {kvp.Value}");
        return sb.ToString();
    }
}
```

**JsonReportRenderer** — format JSON structurat:
```csharp
public sealed class JsonReportRenderer : IReportRenderer
{
    public string Render(string title, IReadOnlyDictionary<string, string> data)
    {
        var report = new { Title = title, GeneratedAt = DateTime.UtcNow.ToString("O"), Data = data };
        return JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
    }
}
```

### Principii SOLID aplicate
- **OCP**: Noi tipuri de rapoarte (CourierPerformanceReport) sau noi renderere (HtmlReportRenderer) se adaugă independent
- **SRP**: Raportul colectează date; renderer-ul formatează output-ul
- **DIP**: DeliveryReport depinde de abstracția IReportRenderer, nu de implementări concrete
- **ISP**: IReportRenderer are o singură metodă, minimă și coezivă

### API Endpoints
- `GET /api/reports/orders?format=console|json` — raport sumar comenzi
- `GET /api/reports/deliveries?format=console|json` — raport status livrări

---

## 4. Proxy Pattern (Protection Proxy)

### Problemă rezolvată

Accesul la repository-ul de comenzi trebuie restricționat pe bază de roluri: doar Admin poate adăuga/modifica/șterge comenzi, iar citirea necesită cel puțin rol de Courier. ProtectionOrderRepositoryProxy implementează aceeași interfață IOrderRepository și verifică rolul curent înaintea fiecărei operații, respingând accesul neautorizat.

### Diagrama UML

```
@startuml
interface IOrderRepository {
  +GetById(id): Order?
  +GetAll(): IEnumerable<Order>
  +GetByCustomerId(customerId): IEnumerable<Order>
  +Add(order): void
  +Update(order): void
  +Delete(id): void
}

interface IAccessContext {
  +GetCurrentRole(): UserRole
}

enum UserRole {
  None
  Courier
  Admin
}

class InMemoryOrderRepository
class ProtectionOrderRepositoryProxy {
  -_realRepository: IOrderRepository
  -_accessContext: IAccessContext
  +Add(order): void  [Admin only]
  +GetById(id): Order?  [Courier+]
  ...
}

class HttpHeaderAccessContext {
  -_httpContextAccessor: IHttpContextAccessor
  +GetCurrentRole(): UserRole
}

IOrderRepository <|.. InMemoryOrderRepository
IOrderRepository <|.. ProtectionOrderRepositoryProxy
ProtectionOrderRepositoryProxy o--> IOrderRepository : _real
ProtectionOrderRepositoryProxy o--> IAccessContext
IAccessContext <|.. HttpHeaderAccessContext
@enduml
```

### Implementare

**ProtectionOrderRepositoryProxy** — proxy cu verificare de roluri:

```csharp
public sealed class ProtectionOrderRepositoryProxy : IOrderRepository
{
    private readonly IOrderRepository _realRepository;
    private readonly IAccessContext _accessContext;

    public void Add(Order order)
    {
        EnsureWriteAccess();
        _realRepository.Add(order);
    }

    public Order? GetById(Guid id)
    {
        EnsureReadAccess();
        return _realRepository.GetById(id);
    }

    private void EnsureReadAccess()
    {
        if (_accessContext.GetCurrentRole() == UserRole.None)
            throw new UnauthorizedAccessException("Authentication required.");
    }

    private void EnsureWriteAccess()
    {
        if (_accessContext.GetCurrentRole() != UserRole.Admin)
            throw new UnauthorizedAccessException("Admin role required.");
    }
}
```

**HttpHeaderAccessContext** — citește rolul din header-ul HTTP `X-User-Role`:

```csharp
public sealed class HttpHeaderAccessContext : IAccessContext
{
    public UserRole GetCurrentRole()
    {
        var headerValue = _httpContextAccessor.HttpContext?.Request.Headers["X-User-Role"].FirstOrDefault();
        return Enum.TryParse<UserRole>(headerValue, ignoreCase: true, out var role) ? role : UserRole.None;
    }
}
```

### Principii SOLID aplicate
- **OCP**: Noi tipuri de proxy (Virtual Proxy, Caching Proxy) se adaugă fără a modifica InMemoryOrderRepository
- **LSP**: Proxy-ul este interschimbabil cu orice IOrderRepository
- **DIP**: Proxy-ul depinde de abstracțiile IOrderRepository și IAccessContext
- **SRP**: Proxy-ul gestionează doar controlul accesului; repository-ul real gestionează doar persistența

### API Endpoints
- `GET /api/orders/protected` — listare comenzi cu verificare rol (header `X-User-Role: Admin|Courier`)
- `GET /api/orders/protected/{id}` — obținere comandă specifică cu verificare rol

---

## Teste Unitare

### FlyweightTests (5 teste)
- `GetZone_SameCode_ReturnsSameInstance` — aceeași instanță pentru același cod
- `GetZone_DifferentCodes_ReturnsDifferentInstances` — instanțe diferite pentru coduri diferite
- `CachedCount_ReflectsUniqueZones` — contorul reflectă zonele unice din cache
- `GetZone_PreservesIntrinsicState` — starea intrinsecă corect conservată
- `GetZone_CaseInsensitiveLookup_ReturnsSameInstance` — lookup case-insensitive

### DecoratorTests (6 teste)
- `LoggingDecorator_LogsBeforeDelegating` — log-ul se emite înainte de delegare
- `SmsDecorator_SendsSmsBeforeDelegating` — SMS-ul se trimite înainte de delegare
- `ChainedDecorators_AllFireInOrder` — LOG → SMS → Inner, ordinea corectă
- `Decorator_NullInner_ThrowsArgumentNullException` — validare null
- `LoggingDecorator_AllMethodsDelegate` — toate cele 5 metode delegă corect
- `SmsDecorator_AllMethodsDelegate` — toate cele 5 metode delegă corect

### BridgeTests (6 teste)
- `OrderSummaryReport_ConsoleRenderer_ProducesTextOutput` — text cu titlu și date
- `OrderSummaryReport_JsonRenderer_ProducesValidJson` — JSON valid cu structură corectă
- `DeliveryStatusReport_ConsoleRenderer_ProducesTextOutput` — text cu statistici livrări
- `DeliveryStatusReport_JsonRenderer_ProducesValidJson` — JSON valid cu total livrări
- `RendererSwappable_SameReportDifferentOutput` — același raport, output diferit per renderer
- `CapturingRenderer_ReceivesCorrectData` — renderer-ul primește titlu și date corecte

### ProxyTests (6 teste)
- `Admin_CanAdd_Order` — Admin poate adăuga comenzi
- `Courier_CannotAdd_ThrowsUnauthorized` — Courier nu poate adăuga (UnauthorizedAccessException)
- `Courier_CanRead_Orders` — Courier poate citi comenzi
- `NoneRole_CannotRead_ThrowsUnauthorized` — None nu poate citi (UnauthorizedAccessException)
- `Proxy_DelegatesToRealRepository` — proxy delegă corect la repository-ul real
- `Admin_CanDelete_Courier_Cannot` — Admin poate șterge, Courier nu

**Total Lab5: 23 teste — toate Passed**

---

## Structura Fișierelor Lab 5

```
DeliverySystem.Domain/
├── Flyweight/
│   ├── DeliveryZone.cs
│   └── DeliveryZoneFactory.cs
└── Enums/
    └── UserRole.cs

DeliverySystem.Interfaces/
├── IAccessContext.cs
└── Reports/
    └── IReportRenderer.cs

DeliverySystem.Infrastructure/
├── AccessContext/
│   └── HttpHeaderAccessContext.cs
├── Notifications/
│   └── Decorators/
│       ├── NotificationServiceDecorator.cs
│       ├── LoggingNotificationDecorator.cs
│       └── SmsNotificationDecorator.cs
├── Reports/
│   ├── ConsoleReportRenderer.cs
│   └── JsonReportRenderer.cs
└── Repositories/
    └── Proxies/
        └── ProtectionOrderRepositoryProxy.cs

DeliverySystem.Services/
└── Reports/
    ├── DeliveryReport.cs
    ├── OrderSummaryReport.cs
    └── DeliveryStatusReport.cs

DeliverySystem.API/
├── Controllers/
│   ├── ReportsController.cs
│   └── ProtectedOrdersController.cs
└── DTOs/
    └── ReportDtos.cs

DeliverySystem.Tests/
└── Lab5/
    ├── FlyweightTests.cs
    ├── DecoratorTests.cs
    ├── BridgeTests.cs
    ├── ProxyTests.cs
    └── TestDoubles/
        ├── RecordingNotificationService.cs
        └── CapturingReportRenderer.cs
```
