# Laborator 7 — Paternuri Comportamentale: Chain of Responsibility, State, Mediator, Template Method, Visitor

## Obiectiv

Implementarea a cinci paternuri comportamentale care îmbunătățesc gestionarea fluxului de control și interacțiunile dintre obiecte:

- **Chain of Responsibility** — pipeline de validare a comenzii
- **State** — automat de stare al livrării, cu o clasă per stare
- **Mediator** — coordonator de dispecerat care decuplează participanții
- **Template Method** — generator de chitanțe cu pași redefinibili
- **Visitor** — operații pe ierarhia de curieri fără a modifica entitățile

Toate cele cinci paternuri au endpoint-uri REST dedicate și pagini în frontend (sidebar „Lab 7 — Behavioral III").

---

## 1. Chain of Responsibility

### Problemă rezolvată

Validarea unei comenzi presupune mai multe verificări succesive (stoc, greutate, distanță, țară, plafon plată). Le-am încapsulat în handleri independenți care comunică printr-un lanț: fiecare poate trece request-ul mai departe sau opri propagarea (StopOnFailure pentru lipsa stocului, care e blocant).

### Diagrama UML

```
@startuml
abstract class OrderValidationHandler {
  -_next: OrderValidationHandler?
  +SetNext(next): OrderValidationHandler
  +Handle(context, result)
  #Process(context, result)
  #StopOnFailure: bool
}

class StockValidationHandler { #StopOnFailure = true }
class WeightLimitHandler
class DistanceHandler
class CountryRestrictionHandler
class PaymentLimitHandler

class OrderValidationPipeline {
  -_entry: OrderValidationHandler
  +Validate(context): OrderValidationResult
  +Default(maxWeight, maxDistance, allowedCountries): pipeline
}

OrderValidationHandler <|-- StockValidationHandler
OrderValidationHandler <|-- WeightLimitHandler
OrderValidationHandler <|-- DistanceHandler
OrderValidationHandler <|-- CountryRestrictionHandler
OrderValidationHandler <|-- PaymentLimitHandler
OrderValidationHandler o--> OrderValidationHandler : _next
OrderValidationPipeline o--> OrderValidationHandler : _entry
@enduml
```

### Implementare

**Base handler** — template fix care decide când propagă:

```csharp
public void Handle(OrderValidationContext context, OrderValidationResult result)
{
    Process(context, result);

    if (!result.IsAccepted && StopOnFailure)
        return;

    _next?.Handle(context, result);
}
```

**StockValidationHandler** — primul în lanț, oprește propagarea când lipsește stocul:

```csharp
protected override bool StopOnFailure => true;

protected override void Process(OrderValidationContext context, OrderValidationResult result)
{
    foreach (var line in context.Lines)
        if (line.Quantity > line.InStock)
        {
            result.Fail(HandlerName, $"'{line.ProductName}' requires {line.Quantity} but only {line.InStock} in stock.");
            return;
        }
    result.Pass(HandlerName, $"{context.Lines.Count} line(s) covered by stock.");
}
```

**Pipeline builder** — wire-up explicit:

```csharp
stock.SetNext(weight).SetNext(distance).SetNext(country).SetNext(payment);
```

### Endpoint

- `POST /api/order-validation` — body `OrderValidationRequest`, returnează passes + failures per handler

### Principii SOLID

- **OCP**: handler nou (ex. fraud detection) se adaugă fără a atinge ceilalți handleri sau pipeline-ul
- **SRP**: fiecare handler verifică o singură regulă
- **DIP**: pipeline-ul cunoaște doar abstracția handler-ului

---

## 2. State Pattern

### Problemă rezolvată

Livrarea are un set finit de stări (Pending, Assigned, PickedUp, InTransit, Delivered, Failed) și fiecare stare permite doar anumite tranziții. În locul unui switch imperativ, fiecare stare e o clasă care decide ce acțiuni acceptă; tranzițiile sunt declarații explicite.

### Diagrama UML

```
@startuml
interface IDeliveryState {
  +Name: string
  +IsTerminal: bool
  +Assign(context)
  +PickUp(context)
  +StartTransit(context)
  +Complete(context)
  +Fail(context, reason)
}

abstract class BaseDeliveryState
class PendingDeliveryState
class AssignedDeliveryState
class PickedUpDeliveryState
class InTransitDeliveryState
class DeliveredDeliveryState <<terminal>>
class FailedDeliveryState <<terminal>>

class DeliveryStateContext {
  -_state: IDeliveryState
  -_trace: List<string>
  +TransitionTo(next)
  +Assign / PickUp / StartTransit / Complete / Fail
}

IDeliveryState <|.. BaseDeliveryState
BaseDeliveryState <|-- PendingDeliveryState
BaseDeliveryState <|-- AssignedDeliveryState
BaseDeliveryState <|-- PickedUpDeliveryState
BaseDeliveryState <|-- InTransitDeliveryState
BaseDeliveryState <|-- DeliveredDeliveryState
BaseDeliveryState <|-- FailedDeliveryState
DeliveryStateContext o--> IDeliveryState
@enduml
```

### Implementare

**BaseDeliveryState** — orice acțiune nepermisă aruncă; derivata override-ază doar ce permite:

```csharp
public abstract class BaseDeliveryState : IDeliveryState
{
    public virtual void Assign(DeliveryStateContext context) => Reject(nameof(Assign));
    public virtual void PickUp(DeliveryStateContext context) => Reject(nameof(PickUp));
    // ... idem
}
```

**PendingDeliveryState** — singura tranziție permisă:

```csharp
public override void Assign(DeliveryStateContext context)
    => context.TransitionTo(new AssignedDeliveryState());
```

**Context** — păstrează un trace al tuturor tranzițiilor pentru debug:

```csharp
public void TransitionTo(IDeliveryState next)
{
    Record($"{_state.Name} -> {next.Name}");
    _state = next;
}
```

### Endpoint-uri

- `GET /api/delivery-state` — snapshot curent + trace
- `POST /api/delivery-state/action` — body `{ "action": "assign" | "pickup" | "transit" | "complete" | "fail", "reason"?: string }`

### Principii SOLID

- **OCP**: o stare nouă (ex. „OnHold") se adaugă fără modificări la stările existente
- **LSP**: orice IDeliveryState e substituibil în context
- **SRP**: fiecare stare definește exclusiv tranzițiile sale legale

---

## 3. Mediator Pattern

### Problemă rezolvată

Coordonarea Order ↔ Courier ↔ Notifier devine un graf cuplat dacă fiecare participant cunoaște ceilalți. Mediator-ul e singurul punct de cunoaștere a topologiei: participanții vorbesc doar cu el, prin mesaje pe topic-uri.

### Diagrama UML

```
@startuml
interface IDispatchMediator {
  +Register(participant)
  +Send(sender, message)
}

interface IDispatchParticipant {
  +Id: string
  +Receive(message)
  +SetMediator(mediator)
}

class DispatchMediator {
  -_participants: Dictionary<string, IDispatchParticipant>
  -_log: List<string>
}

abstract class BaseDispatchParticipant
class OrderDispatchParticipant
class CourierDispatchParticipant
class NotifierDispatchParticipant

IDispatchMediator <|.. DispatchMediator
IDispatchParticipant <|.. BaseDispatchParticipant
BaseDispatchParticipant <|-- OrderDispatchParticipant
BaseDispatchParticipant <|-- CourierDispatchParticipant
BaseDispatchParticipant <|-- NotifierDispatchParticipant
DispatchMediator o--> IDispatchParticipant
@enduml
```

### Flux de mesaje

1. `Order.RequestDispatch()` → `order.ready` (broadcast)
2. `Courier` primește, trimite `courier.assigned`
3. `Order` primește `courier.assigned` → trimite `order.acknowledge`
4. `Notifier` ascultă `courier.assigned` și produce mesaj pentru client

Niciun participant nu deține o referință la altul — `Send(sender, message)` e singurul canal.

### Implementare

**Mediator** — broadcast-ul exclude sender-ul:

```csharp
public void Send(IDispatchParticipant sender, DispatchMessage message)
{
    if (message.TargetId != null)
    {
        if (_participants.TryGetValue(message.TargetId, out var direct))
            direct.Receive(message);
        return;
    }

    foreach (var (id, participant) in _participants)
    {
        if (id == sender.Id) continue;
        participant.Receive(message);
    }
}
```

### Endpoint-uri

- `GET /api/mediator` — snapshot (registered, log, notifier emitted)
- `POST /api/mediator/register` — body `{ "kind": "order" | "courier", "name": "…" }`
- `POST /api/mediator/dispatch?orderId=…`

### Principii SOLID

- **DIP**: participanții depind de abstracția mediator
- **OCP**: tipuri noi de participanți se adaugă fără modificarea celor existenți
- **SRP**: mediator-ul gestionează exclusiv routing-ul

---

## 4. Template Method

### Problemă rezolvată

Orice chitanță are aceeași structură (header → lines → summary → metadata? → footer), dar conținutul fiecărui pas diferă. `ReceiptGenerator.Generate()` e algoritmul-șablon, iar subclasele implementează doar pașii specifici. Hook-ul `IncludeMetadata` permite secțiuni opționale.

### Diagrama UML

```
@startuml
abstract class ReceiptGenerator {
  +Generate(): string  <<template method>>
  #WriteHeader(sb)*
  #WriteLines(sb)*
  #WriteSummary(sb)*
  #WriteFooter(sb)
  #IncludeMetadata: bool
  #WriteMetadata(sb)
}

class OrderReceiptGenerator { #IncludeMetadata = true }
class DeliveryReceiptGenerator
class RefundReceiptGenerator

ReceiptGenerator <|-- OrderReceiptGenerator
ReceiptGenerator <|-- DeliveryReceiptGenerator
ReceiptGenerator <|-- RefundReceiptGenerator
@enduml
```

### Implementare

**Algoritmul șablon (fix)**:

```csharp
public string Generate()
{
    var sb = new StringBuilder();
    WriteHeader(sb);
    sb.AppendLine();
    WriteLines(sb);
    sb.AppendLine();
    WriteSummary(sb);

    if (IncludeMetadata)
    {
        sb.AppendLine();
        WriteMetadata(sb);
    }

    sb.AppendLine();
    WriteFooter(sb);
    return sb.ToString();
}
```

**OrderReceiptGenerator** — activează hook-ul de metadate:

```csharp
protected override bool IncludeMetadata => true;

protected override void WriteMetadata(StringBuilder sb)
{
    sb.AppendLine($"Created at: {_order.CreatedAt:O}");
    if (_order.UpdatedAt.HasValue)
        sb.AppendLine($"Updated at: {_order.UpdatedAt:O}");
}
```

**DeliveryReceiptGenerator** — override la footer:

```csharp
protected override void WriteFooter(StringBuilder sb)
{
    sb.AppendLine("Thank you for using our courier service.");
    sb.AppendLine("-- end of delivery receipt --");
}
```

### Endpoint

- `POST /api/receipts` — body `{ "kind": "order" | "delivery" | "refund", "id": "…", … }`

### Principii SOLID

- **OCP**: alt tip de chitanță (TaxReceipt) se adaugă ca derivată nouă
- **LSP**: orice ReceiptGenerator produce text bine format prin Generate()
- **DRY**: algoritmul comun e implementat o singură dată

---

## 5. Visitor

### Problemă rezolvată

Avem ierarhia `Courier → BikeCourier / CarCourier / DroneCourier`. Vrem să rulăm operații (capacitate descrisă, cost de mentenanță, scor eco) fără a adăuga metode noi în entități. Visitor-ul plus double-dispatch (prin extension method `Accept`) realizează asta cu siguranță la compilare — adăugarea unei subclase noi forțează modificarea interfeței `ICourierVisitor<TResult>`.

### Diagrama UML

```
@startuml
interface ICourierVisitor<TResult> {
  +Visit(BikeCourier): TResult
  +Visit(CarCourier): TResult
  +Visit(DroneCourier): TResult
}

class CourierVisitorExtensions {
  +Accept<TResult>(courier, visitor): TResult  <<double dispatch>>
}

class CapacityReportVisitor : ICourierVisitor<string>
class MaintenanceCostVisitor : ICourierVisitor<decimal>
class EcoScoreVisitor : ICourierVisitor<int>

abstract class Courier
class BikeCourier
class CarCourier
class DroneCourier

Courier <|-- BikeCourier
Courier <|-- CarCourier
Courier <|-- DroneCourier
ICourierVisitor <|.. CapacityReportVisitor
ICourierVisitor <|.. MaintenanceCostVisitor
ICourierVisitor <|.. EcoScoreVisitor
@enduml
```

### Implementare

**Accept extension** — alege overload-ul corect prin pattern matching:

```csharp
public static TResult Accept<TResult>(this Courier courier, ICourierVisitor<TResult> visitor)
{
    return courier switch
    {
        BikeCourier bike => visitor.Visit(bike),
        CarCourier car => visitor.Visit(car),
        DroneCourier drone => visitor.Visit(drone),
        _ => throw new NotSupportedException($"Visitor does not handle '{courier.GetType().Name}'.")
    };
}
```

**MaintenanceCostVisitor** — costul depinde de subtipul concret și starea instanței:

```csharp
public decimal Visit(DroneCourier courier) => 90m + courier.MaxFlightRangeKm * 0.50m;
```

### Endpoint

- `POST /api/courier-visitor` — body `{ "visitor": "capacity" | "maintenance" | "eco" }`

### Principii SOLID

- **OCP** (pe operații): un visitor nou se adaugă fără a modifica entitățile sau celelalte visitor-uri
- **SRP**: fiecare visitor implementează exact un singur algoritm
- **DIP**: codul client lucrează prin `ICourierVisitor<TResult>`

---

## Teste Unitare

Fișiere în `DeliverySystem.Tests/Lab7/`:

| Fișier                        | Teste | Acoperire                                                  |
|-------------------------------|-------|------------------------------------------------------------|
| `ChainOfResponsibilityTests`  | 8     | happy path, short-circuit, weight/distance/country/wallet  |
| `StateTests`                  | 9     | secvență validă, acțiuni respinse, fail tranziții          |
| `MediatorTests`               | 7     | broadcast, no-echo, direct routing, log, registrare        |
| `TemplateMethodTests`         | 7     | hooks, footer override, validări, structură stabilă        |
| `VisitorTests`                | 7     | double dispatch, rezultate per subtip, multiple result types |
| **Total Lab 7**               | **38**| toate **Passed**                                            |

Suita completă a proiectului: **174 teste — toate Passed.**

---

## Pagini Frontend

| Rută                | Patern                  | Descriere                                                  |
|---------------------|-------------------------|------------------------------------------------------------|
| `/validation`       | Chain of Responsibility | Editor linii + parametri, vizualizare passes / failures    |
| `/state`            | State                   | Diagramă stări curentă, butoane acțiuni, trace             |
| `/mediator`         | Mediator                | Înregistrare participanți, dispatch, log, notificări       |
| `/receipts`         | Template Method         | Picker pentru order / delivery / refund + body formatat    |
| `/courier-visitor`  | Visitor                 | Selector visitor + tabel cu rezultate per curier           |

---

## Structura Fișierelor Lab 7

```
DeliverySystem.Domain/
├── Chain/
│   ├── OrderValidationContext.cs   (record)
│   ├── OrderValidationHandler.cs   (abstract)
│   ├── StockValidationHandler.cs   (StopOnFailure = true)
│   ├── WeightLimitHandler.cs
│   ├── DistanceHandler.cs
│   ├── CountryRestrictionHandler.cs
│   └── PaymentLimitHandler.cs
├── States/
│   ├── IDeliveryState.cs
│   ├── BaseDeliveryState.cs
│   ├── DeliveryStateContext.cs
│   └── PendingDeliveryState.cs / Assigned / PickedUp / InTransit / Delivered / Failed
├── Mediator/
│   ├── IDispatchMediator.cs
│   ├── BaseDispatchParticipant.cs
│   └── OrderDispatchParticipant.cs / Courier / Notifier
├── Templates/
│   ├── ReceiptGenerator.cs   (abstract)
│   ├── OrderReceiptGenerator.cs
│   ├── DeliveryReceiptGenerator.cs
│   └── RefundReceiptGenerator.cs
└── Visitors/
    ├── ICourierVisitor.cs
    ├── CourierVisitorExtensions.cs
    ├── CapacityReportVisitor.cs
    ├── MaintenanceCostVisitor.cs
    └── EcoScoreVisitor.cs

DeliverySystem.Services/
├── Chain/OrderValidationPipeline.cs
└── Mediator/DispatchMediator.cs

DeliverySystem.API/
├── Controllers/
│   ├── OrderValidationController.cs
│   ├── DeliveryStateController.cs
│   ├── DispatchMediatorController.cs
│   ├── ReceiptController.cs
│   └── CourierVisitorController.cs
└── DTOs/Lab7Dtos.cs

DeliverySystem.Tests/Lab7/
├── ChainOfResponsibilityTests.cs
├── StateTests.cs
├── MediatorTests.cs
├── TemplateMethodTests.cs
└── VisitorTests.cs
```

---

## Concluzii

Cele cinci paterne din Lab 7 elimină cuplaje rigide care altfel s-ar fi acumulat:

- **Chain of Responsibility**: regulile de validare sunt acum „LEGO" — recombinabile fără a atinge nimic existent
- **State**: tranzițiile sunt obiecte explicite; un compilator care vede `Reject(...)` în baza forțează gândirea înainte de a adăuga acțiuni
- **Mediator**: graful de dependențe între Order/Courier/Notifier dispare; mediator-ul e singurul nod
- **Template Method**: structura chitanței e fixă, conținutul flexibil — DRY garantat la nivel de algoritm
- **Visitor**: noi rapoarte despre flotă (cost mediu, scor eco, eligibilitate task) se adaugă fără a atinge clasele de curier
