# Laborator 6 — Paternuri Comportamentale: Strategy, Observer, Command, Memento, Iterator

## Obiectiv

Implementarea a cinci paternuri comportamentale în cadrul Delivery Management System, separând logica aplicației de comportamente specifice pentru a obține flexibilitate și modularitate:

- **Strategy** — schimbarea dinamică a algoritmului de calcul al costului de livrare
- **Observer** — notificarea automată a abonaților la schimbări de stare ale livrărilor
- **Command** — încapsularea operațiilor de dispecerat cu suport Undo/Redo
- **Memento** — capturarea și restaurarea stării unei comenzi-draft
- **Iterator** — parcurgerea unei colecții de curieri în mai multe ordini, fără a expune structura

Toate cele cinci paternuri sunt expuse atât prin endpoint-uri REST cât și prin pagini dedicate în frontend (sidebar „Lab 6 — Behavioral II").

---

## 1. Strategy Pattern

### Problemă rezolvată

Sistemul oferă mai multe niveluri de servicii (Standard, Express, Economy), fiecare cu propria formulă de cost și propria estimare de timp. Hardcodarea unei singure formule ar bloca evoluția pricing-ului. Strategy permite schimbarea algoritmului la runtime fără a modifica restul aplicației.

### Diagrama UML

```
@startuml
interface IDeliveryCostStrategy {
  +StrategyName: string
  +CalculateCost(distanceKm, weightKg): decimal
  +EstimatedDeliveryTime(distanceKm): TimeSpan
}

class StandardDeliveryCostStrategy
class ExpressDeliveryCostStrategy
class EconomyDeliveryCostStrategy

class DeliveryCostCalculator {
  -_strategy: IDeliveryCostStrategy
  +SetStrategy(strategy)
  +Quote(distanceKm, weightKg): DeliveryCostQuote
}

IDeliveryCostStrategy <|.. StandardDeliveryCostStrategy
IDeliveryCostStrategy <|.. ExpressDeliveryCostStrategy
IDeliveryCostStrategy <|.. EconomyDeliveryCostStrategy
DeliveryCostCalculator o--> IDeliveryCostStrategy
@enduml
```

### Implementare

**IDeliveryCostStrategy** — contract minimal:

```csharp
public interface IDeliveryCostStrategy
{
    string StrategyName { get; }
    decimal CalculateCost(decimal distanceKm, decimal weightKg);
    TimeSpan EstimatedDeliveryTime(decimal distanceKm);
}
```

**ExpressDeliveryCostStrategy** — formulă cu multiplicator de prioritate:

```csharp
public sealed class ExpressDeliveryCostStrategy : IDeliveryCostStrategy
{
    public string StrategyName => "Express";

    public decimal CalculateCost(decimal distanceKm, decimal weightKg)
    {
        var raw = 12.00m + (distanceKm * 1.50m) + (weightKg * 0.80m);
        return Math.Round(raw * 1.35m, 2);
    }
}
```

**DeliveryCostCalculator** — context care primește strategy-ul și îl poate înlocui:

```csharp
var calc = new DeliveryCostCalculator(new StandardDeliveryCostStrategy());
var standardQuote = calc.Quote(10m, 2m);   // 14.00 RON

calc.SetStrategy(new ExpressDeliveryCostStrategy());
var expressQuote = calc.Quote(10m, 2m);    // 38.61 RON
```

### Endpoint-uri

- `GET /api/delivery-quote/strategies` — listează strategiile disponibile
- `POST /api/delivery-quote` — quote cu strategy selectat
- `POST /api/delivery-quote/compare` — quote pe toate strategiile pentru aceiași parametri (afișat în UI ca trei carduri)

### Principii SOLID

- **OCP**: o strategie nouă (ex. „Subscription") se adaugă fără a modifica calculatorul
- **SRP**: fiecare strategy capsulează exact o formulă; calculatorul doar deleagă
- **DIP**: calculatorul depinde de interfață, nu de implementări concrete

---

## 2. Observer Pattern

### Problemă rezolvată

O livrare își schimbă statusul (Pending → Assigned → PickedUp → InTransit → Delivered). Mai mulți consumatori vor să știe — clientul prin email, prin SMS și un dashboard intern. Cuplarea directă a livrării de toți acești consumatori ar fi rigidă; Observer permite atașarea/detașarea dinamică a abonaților.

### Diagrama UML

```
@startuml
interface IDeliveryObserver {
  +ChannelName: string
  +OnDeliveryStatusChanged(delivery, previousStatus)
}

interface IDeliverySubject {
  +Attach(observer)
  +Detach(observer)
  +Notify(delivery, previousStatus)
}

class DeliveryStatusSubject {
  -_observers: List<IDeliveryObserver>
  +Attach / Detach / Notify
}

class EmailDeliveryObserver
class SmsDeliveryObserver
class DashboardDeliveryObserver

IDeliverySubject <|.. DeliveryStatusSubject
IDeliveryObserver <|.. EmailDeliveryObserver
IDeliveryObserver <|.. SmsDeliveryObserver
IDeliveryObserver <|.. DashboardDeliveryObserver
DeliveryStatusSubject o--> IDeliveryObserver : notifies
@enduml
```

### Implementare

**DeliveryStatusSubject** — listă thread-safe, snapshot la notificare:

```csharp
public void Notify(Delivery delivery, DeliveryStatus previousStatus)
{
    IDeliveryObserver[] snapshot;
    lock (_gate) snapshot = _observers.ToArray();

    foreach (var observer in snapshot)
        observer.OnDeliveryStatusChanged(delivery, previousStatus);
}
```

**SmsDeliveryObserver** — emite SMS doar pentru tranzițiile critice:

```csharp
public void OnDeliveryStatusChanged(Delivery delivery, DeliveryStatus previousStatus)
{
    if (delivery.Status is DeliveryStatus.Delivered
        or DeliveryStatus.Failed
        or DeliveryStatus.InTransit)
    {
        _send(_phone, $"Update: {delivery.Status} (was {previousStatus}).");
    }
}
```

### Endpoint-uri

- `GET /api/delivery-events/channels` — abonații curenți
- `POST /api/delivery-events/subscribe-email?email=…`
- `POST /api/delivery-events/subscribe-sms?phone=…`
- `POST /api/delivery-events/simulate-status?deliveryId=…&action=assign|pickup|transit|deliver`
- `GET /api/delivery-events` — feed înregistrat de DashboardObserver

### Principii SOLID

- **OCP**: noi tipuri de observer (Slack, Webhook) se adaugă fără a atinge subiectul
- **DIP**: subiectul lucrează cu abstracția observatorului
- **SRP**: fiecare observer are un singur canal de comunicare

---

## 3. Command Pattern

### Problemă rezolvată

Operațiile de dispecerat (Assign, PickUp, Transit, Complete) trebuie să poată fi anulate (Undo) și re-executate (Redo). Encapsularea fiecărei operații într-un obiect Command permite stocarea istoricului și execuția deferită. Macro-Command-ul rulează un set scriptat de pași într-o singură unitate.

### Diagrama UML

```
@startuml
interface IDeliveryCommand {
  +Description: string
  +Execute()
  +Undo()
}

class AssignCourierCommand
class PickUpDeliveryCommand
class StartTransitCommand
class CompleteDeliveryCommand
class MacroDeliveryCommand {
  -_commands: IReadOnlyList<IDeliveryCommand>
}

class DeliveryCommandInvoker {
  -_undoStack: Stack<IDeliveryCommand>
  -_redoStack: Stack<IDeliveryCommand>
  +Execute(command)
  +Undo()
  +Redo()
}

IDeliveryCommand <|.. AssignCourierCommand
IDeliveryCommand <|.. PickUpDeliveryCommand
IDeliveryCommand <|.. StartTransitCommand
IDeliveryCommand <|.. CompleteDeliveryCommand
IDeliveryCommand <|.. MacroDeliveryCommand
DeliveryCommandInvoker o--> IDeliveryCommand
MacroDeliveryCommand o--> IDeliveryCommand : children
@enduml
```

### Implementare

**Comandă concretă** — capturează starea anterioară pentru undo:

```csharp
public sealed class PickUpDeliveryCommand : IDeliveryCommand
{
    private DeliveryStatus _previousStatus;

    public void Execute()
    {
        _previousStatus = _delivery.Status;
        _delivery.MarkAsPickedUp();
    }

    public void Undo() => _delivery.RewindStatus(_previousStatus);
}
```

**Invoker** — stive de undo/redo + history:

```csharp
public void Execute(IDeliveryCommand command)
{
    command.Execute();
    _undoStack.Push(command);
    _redoStack.Clear();
}

public void Undo()
{
    var command = _undoStack.Pop();
    command.Undo();
    _redoStack.Push(command);
}
```

### Endpoint-uri

- `POST /api/dispatch/execute` — body `{ "deliveryId": "…", "action": "assign" }`
- `POST /api/dispatch/undo`
- `POST /api/dispatch/redo`
- `GET /api/dispatch/history`
- `POST /api/dispatch/clear`

### Principii SOLID

- **OCP**: o comandă nouă (de ex. `RescheduleDeliveryCommand`) nu cere modificări la invoker
- **SRP**: invoker-ul nu cunoaște logica fiecărei operații; doar le programează
- **LSP**: orice IDeliveryCommand este interschimbabil într-un macro sau în invoker

---

## 4. Memento Pattern

### Problemă rezolvată

Un client construiește o comandă pas cu pas (adaugă produse, ajustează prioritatea, adaugă note). Vrea să salveze versiuni intermediare („after-pizza", „before-discount") și să poată reveni la oricare. Memento încapsulează starea draftului într-un obiect imuabil pe care doar originatorul îl poate „citi" intern, iar caretaker-ul îl ține în istoric fără să-i cunoască conținutul.

### Diagrama UML

```
@startuml
class OrderDraft <<originator>> {
  -_lines: List<OrderDraftLine>
  -_priority: OrderPriority
  -_deliveryNotes: string?
  +AddLine / RemoveLineAt / SetPriority
  +Save(label): OrderDraftMemento
  +Restore(memento)
}

class OrderDraftMemento <<memento>> {
  +Label: string
  +SavedAt: DateTime
  ~Lines / Priority / DeliveryNotes
}

class OrderDraftCaretaker <<caretaker>> {
  -_history: List<OrderDraftMemento>
  +Push / FindByLabel / PopLast / Clear
}

OrderDraft ..> OrderDraftMemento : creates / restores
OrderDraftCaretaker o--> OrderDraftMemento : stores
@enduml
```

### Implementare

Câmpurile interne ale memento-ului sunt `internal`, deci numai `OrderDraft` (din același assembly) le poate citi:

```csharp
public sealed class OrderDraftMemento
{
    internal IReadOnlyList<OrderDraftLine> Lines { get; }
    internal OrderPriority Priority { get; }
    internal string? DeliveryNotes { get; }
    public string Label { get; }
    public DateTime SavedAt { get; }
}
```

Save / Restore — creează un snapshot și îl reaplică:

```csharp
public OrderDraftMemento Save(string label) =>
    new(label, _lines, _priority, _deliveryNotes);

public void Restore(OrderDraftMemento memento)
{
    _lines.Clear();
    _lines.AddRange(memento.Lines);
    _priority = memento.Priority;
    _deliveryNotes = memento.DeliveryNotes;
}
```

### Endpoint-uri

- `GET /api/order-draft` — starea curentă
- `POST /api/order-draft/lines` — adaugă linie
- `DELETE /api/order-draft/lines/{index}`
- `POST /api/order-draft/priority/{priority}`
- `POST /api/order-draft/save` — body `{ "label": "v1" }`
- `GET /api/order-draft/snapshots`
- `POST /api/order-draft/restore` — body `{ "label": "v1" }`

### Principii SOLID

- **SRP**: originator-ul gestionează state-ul curent; caretaker-ul doar stochează snapshots; memento-ul doar le poartă
- **OCP**: snapshots viitoare cu metadate suplimentare se adaugă fără a strica caretaker-ul
- **Encapsulare**: caretaker-ul nu poate citi datele din memento, doar le mută

---

## 5. Iterator Pattern

### Problemă rezolvată

Sistemul are o colecție de curieri pe care vrem să o parcurgem în mai multe moduri: ordine de inserție, doar cei disponibili, doar de un anumit tip, sau round-robin pentru repartizare echitabilă. Iterator-ul standard expune aceste moduri sub o interfață uniformă, fără ca apelantul să cunoască structura internă.

### Diagrama UML

```
@startuml
interface IDeliveryIterator<T> {
  +HasNext(): bool
  +Next(): T
  +Reset()
}

class CourierCollection {
  -_couriers: List<Courier>
  +CreateInsertionOrderIterator()
  +CreateAvailableIterator()
  +CreateVehicleTypeIterator(vehicleType)
  +CreateRoundRobinIterator(totalSteps)
}

class InsertionOrderCourierIterator
class AvailableCourierIterator
class VehicleTypeCourierIterator
class RoundRobinCourierIterator

IDeliveryIterator <|.. InsertionOrderCourierIterator
IDeliveryIterator <|.. AvailableCourierIterator
IDeliveryIterator <|.. VehicleTypeCourierIterator
IDeliveryIterator <|.. RoundRobinCourierIterator
CourierCollection ..> IDeliveryIterator : factory methods
@enduml
```

### Implementare

**RoundRobinCourierIterator** — wrap-around pentru un număr fix de pași:

```csharp
public Courier Next()
{
    var courier = _items[_cursor];
    _cursor = (_cursor + 1) % _items.Count;
    _stepsTaken++;
    return courier;
}
```

**CourierCollection** — aggregate cu factory methods, ascunde lista internă:

```csharp
public IDeliveryIterator<Courier> CreateAvailableIterator()
    => new AvailableCourierIterator(_couriers);
```

### Endpoint

- `POST /api/courier-iterator` — body `{ "mode": "round-robin", "roundRobinSteps": 7 }` (mode ∈ insertion / available / vehicle / round-robin)

### Principii SOLID

- **SRP**: aggregate-ul stochează; iterator-ul parcurge
- **OCP**: o nouă strategie de parcurgere se adaugă ca un nou iterator, fără a atinge colecția
- **DIP**: codul client lucrează doar cu `IDeliveryIterator<T>`

---

## Teste Unitare

Fișiere în `DeliverySystem.Tests/Lab6/`:

| Fișier             | Teste | Acoperire                                      |
|--------------------|-------|------------------------------------------------|
| `StrategyTests`    | 8     | calcul cost / ETA / runtime swap / validări    |
| `ObserverTests`    | 7     | attach/detach, filtre per canal, no-observers  |
| `CommandTests`     | 9     | execute / undo / redo / macro / history        |
| `MementoTests`     | 8     | save / restore / caretaker rewind / immutable  |
| `IteratorTests`    | 8     | toate cele 4 moduri + reset + exhaustion       |
| **Total Lab 6**    | **40**| toate **Passed**                               |

Suita completă a proiectului: **136 teste — toate Passed.**

---

## Pagini Frontend

| Rută               | Patern    | Descriere                                                  |
|--------------------|-----------|------------------------------------------------------------|
| `/quote`           | Strategy  | Selector strategy, Quote / Compare All                     |
| `/events`          | Observer  | Abonare email/SMS, simulare tranziții, feed dashboard      |
| `/dispatch`        | Command   | Acțiuni pe livrare + Undo / Redo + history                 |
| `/draft`           | Memento   | Editor draft + listă snapshot-uri + Restore                |
| `/courier-walk`    | Iterator  | Mode picker (4 moduri) + lista curierilor parcurși         |

---

## Structura Fișierelor Lab 6

```
DeliverySystem.Domain/
├── Strategies/
│   ├── IDeliveryCostStrategy.cs
│   ├── StandardDeliveryCostStrategy.cs
│   ├── ExpressDeliveryCostStrategy.cs
│   └── EconomyDeliveryCostStrategy.cs
├── Commands/
│   ├── IDeliveryCommand.cs
│   ├── AssignCourierCommand.cs
│   ├── PickUpDeliveryCommand.cs
│   ├── StartTransitCommand.cs
│   ├── CompleteDeliveryCommand.cs
│   └── MacroDeliveryCommand.cs
├── Memento/
│   ├── OrderDraftMemento.cs
│   └── OrderDraft.cs
└── Iterators/
    ├── IDeliveryIterator.cs
    ├── CourierCollection.cs
    ├── InsertionOrderCourierIterator.cs
    ├── AvailableCourierIterator.cs
    ├── VehicleTypeCourierIterator.cs
    └── RoundRobinCourierIterator.cs

DeliverySystem.Interfaces/
└── Observer/
    ├── IDeliveryObserver.cs
    └── IDeliverySubject.cs

DeliverySystem.Infrastructure/
└── Observer/
    ├── DeliveryStatusSubject.cs
    ├── EmailDeliveryObserver.cs
    ├── SmsDeliveryObserver.cs
    └── DashboardDeliveryObserver.cs

DeliverySystem.Services/
├── DeliveryCostCalculator.cs
├── Commands/
│   └── DeliveryCommandInvoker.cs
└── Memento/
    └── OrderDraftCaretaker.cs

DeliverySystem.API/
├── Controllers/
│   ├── DeliveryQuoteController.cs
│   ├── DeliveryEventsController.cs
│   ├── DispatchCommandController.cs
│   ├── OrderDraftController.cs
│   └── CourierIteratorController.cs
└── DTOs/
    └── Lab6Dtos.cs

DeliverySystem.Tests/Lab6/
├── StrategyTests.cs
├── ObserverTests.cs
├── CommandTests.cs
├── MementoTests.cs
└── IteratorTests.cs
```

---

## Concluzii

Cele cinci paterne comportamentale au fost integrate fără modificări intruzive în arhitectura existentă: fiecare patern stă în propriul namespace, depinde de abstracții, și expune o suprafață stabilă atât pentru API cât și pentru UI. Beneficiile cheie:

- **Strategy**: schimbarea regulilor de pricing se face declarativ, cu un singur DI swap
- **Observer**: noi canale de notificare pot fi atașate la rulare fără redeploy de logică core
- **Command**: undo/redo a devenit parte din contractul fiecărei acțiuni; macro-flows sunt o compoziție de comenzi
- **Memento**: draftul de comandă poate fi versionat fără ca structura sa internă să fie expusă
- **Iterator**: politici noi de parcurgere se adaugă fără a atinge `CourierCollection`
