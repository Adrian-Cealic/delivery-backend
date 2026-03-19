# Laborator 4 — Paternuri Structurale: Adapter, Composite, Façade

## Obiectiv

Implementarea a trei paternuri structurale în cadrul Delivery Management System:
- **Adapter** — compatibilitatea între clase cu interfețe diferite (gateway-uri de plată)
- **Composite** — model ierarhic pentru obiecte individuale și colecții tratate uniform (catalog produse)
- **Façade** — interfață simplificată către subsistem complex (plasare comandă)

---

## 1. Adapter Pattern

### Problemă rezolvată

Sistemul trebuie să integreze multiple gateway-uri de plată (PayPal, Stripe, Google Pay) cu API-uri externe care au semnături și convenții diferite. Fără un adapter, fiecare consumator ar trebui să cunoască detaliile fiecărui API. Adapter-ul standardizează toate gateway-urile printr-o interfață comună `IPaymentGateway`, permitând utilizarea unitară în PaymentService și OrderPlacementFacade.

### Diagrama UML

```
@startuml
interface IPaymentGateway {
  +ProcessPayment(amount, currency, referenceId): PaymentResult
}

package "Adapters" {
  class PayPalPaymentAdapter
  class StripePaymentAdapter
  class GooglePayPaymentAdapter
}

package "External APIs" {
  class PayPalApi {
    +SendPaymentRequest(token, amount, ref): PayPalPaymentResponse
  }
  class StripeApi {
    +Charge(key, amountInCents, idempotencyKey): StripeChargeResponse
  }
  class GooglePayApi {
    +ProcessPayment(token, request): GooglePayResponse
  }
}

IPaymentGateway <|.. PayPalPaymentAdapter
IPaymentGateway <|.. StripePaymentAdapter
IPaymentGateway <|.. GooglePayPaymentAdapter

PayPalPaymentAdapter o--> PayPalApi
StripePaymentAdapter o--> StripeApi
GooglePayPaymentAdapter o--> GooglePayApi
@enduml
```

### Implementare

**IPaymentGateway** — interfață comună (target):

```csharp
public interface IPaymentGateway
{
    PaymentResult ProcessPayment(decimal amount, string currency, string referenceId);
}

public record PaymentResult(bool Success, string? TransactionId, string? ErrorMessage);
```

**Clase adaptate** (adaptees) — API-uri simulate cu semnături diferite:

```csharp
public sealed class PayPalApi
{
    public PayPalPaymentResponse SendPaymentRequest(string paypalToken, decimal amount, string reference)
    {
        var transactionId = $"paypal_{Guid.NewGuid():N}";
        return new PayPalPaymentResponse(true, transactionId, null);
    }
}

public sealed class StripeApi
{
    public StripeChargeResponse Charge(string stripeKey, long amountInCents, string idempotencyKey)
    {
        var transactionId = $"ch_{Guid.NewGuid():N}";
        return new StripeChargeResponse(true, transactionId, null);
    }
}

public sealed class GooglePayApi
{
    public GooglePayResponse ProcessPayment(string walletToken, GooglePayPaymentRequest request)
    {
        var transactionId = $"gpay_{Guid.NewGuid():N}";
        return new GooglePayResponse(true, transactionId, null);
    }
}
```

**PayPalPaymentAdapter** — adaptează API-ul PayPal la IPaymentGateway:

```csharp
public sealed class PayPalPaymentAdapter : IPaymentGateway
{
    private readonly PayPalApi _payPalApi;

    public PaymentResult ProcessPayment(decimal amount, string currency, string referenceId)
    {
        var response = _payPalApi.SendPaymentRequest(_defaultToken, amount, referenceId);
        return new PaymentResult(response.Success, response.TransactionId, response.ErrorMessage);
    }
}
```

**PaymentGatewayProvider** — furnizează adapter-ul corespunzător tipului selectat:

```csharp
public IPaymentGateway GetGateway(PaymentGatewayType type) =>
    type switch
    {
        PaymentGatewayType.PayPal => _payPalAdapter,
        PaymentGatewayType.Stripe => _stripeAdapter,
        PaymentGatewayType.GooglePay => _googlePayAdapter,
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
```

### Principii SOLID aplicate
- **OCP**: Adăugarea unui nou gateway (ex. Apple Pay) necesită doar un adapter nou, fără modificarea PaymentService
- **DIP**: PaymentService și OrderPlacementFacade depind de IPaymentGateway, nu de implementări concrete
- **SRP**: Fiecare adapter transformă un singur API extern în interfața comună

### API Endpoint
- `POST /api/payments/process` — procesează plată cu gateway selectat (PayPal, Stripe, GooglePay)

---

## 2. Composite Pattern

### Problemă rezolvată

Catalogul de produse trebuie să reprezinte atât produse individuale (cafea, pizza) cât și categorii/combinații (meniu zilnic, combo meal). Fără Composite, codul ar trata leaf-urile și containerele diferit, duplicând logică. Composite permite tratarea uniformă: `GetTotalPrice()`, `GetTotalWeight()` și `GetChildren()` funcționează la fel pentru un produs simplu și pentru un bundle de produse.

### Diagrama UML

```
@startuml
interface IProductCatalogComponent {
  +Name: string
  +GetTotalPrice(): decimal
  +GetTotalWeight(): decimal
  +GetChildren(): IReadOnlyList
  +ToOrderItems(quantity): IEnumerable<OrderItem>
}

class ProductCatalogItem {
  +Name: string
  +UnitPrice: decimal
  +Weight: decimal
  +GetTotalPrice(): decimal
  +GetTotalWeight(): decimal
  +GetChildren(): empty
  +ToOrderItems(quantity): OrderItem
}

class ProductBundle {
  +Name: string
  -_children: List<IProductCatalogComponent>
  +Add(component): void
  +Remove(component): void
  +GetTotalPrice(): sum of children
  +GetTotalWeight(): sum of children
  +GetChildren(): _children
  +ToOrderItems(quantity): flattened items
}

IProductCatalogComponent <|.. ProductCatalogItem
IProductCatalogComponent <|.. ProductBundle
ProductBundle o-- IProductCatalogComponent
@enduml
```

### Implementare

**IProductCatalogComponent** — interfață comună pentru leaf și composite:

```csharp
public interface IProductCatalogComponent
{
    string Name { get; }
    decimal GetTotalPrice();
    decimal GetTotalWeight();
    IReadOnlyList<IProductCatalogComponent> GetChildren();
    IEnumerable<OrderItem> ToOrderItems(int quantity = 1);
}
```

**ProductCatalogItem** (leaf) — produs simplu:

```csharp
public sealed class ProductCatalogItem : IProductCatalogComponent
{
    public string Name { get; }
    public decimal UnitPrice { get; }
    public decimal Weight { get; }

    public IReadOnlyList<IProductCatalogComponent> GetChildren() =>
        Array.Empty<IProductCatalogComponent>();

    public decimal GetTotalPrice() => UnitPrice;
    public decimal GetTotalWeight() => Weight;

    public IEnumerable<OrderItem> ToOrderItems(int quantity = 1)
    {
        yield return new OrderItem(Name, quantity, UnitPrice, Weight);
    }
}
```

**ProductBundle** (composite) — colecție de componente:

```csharp
public sealed class ProductBundle : IProductCatalogComponent
{
    private readonly List<IProductCatalogComponent> _children;

    public decimal GetTotalPrice() => _children.Sum(c => c.GetTotalPrice());
    public decimal GetTotalWeight() => _children.Sum(c => c.GetTotalWeight());
    public IReadOnlyList<IProductCatalogComponent> GetChildren() => _children.AsReadOnly();

    public IEnumerable<OrderItem> ToOrderItems(int quantity = 1)
    {
        foreach (var child in _children)
            foreach (var item in child.ToOrderItems(quantity))
                yield return item;
    }
}
```

**CatalogProvider** — construiește un catalog exemplu ierarhic:

```csharp
var combo = new ProductBundle("Combo Meal");
combo.Add(new ProductCatalogItem("Burger", 12.50m, 0.4m));
combo.Add(new ProductCatalogItem("Fries", 4.00m, 0.2m));
combo.Add(new ProductCatalogItem("Soft Drink", 2.50m, 0.3m));

var dailyMenu = new ProductBundle("Daily Menu");
dailyMenu.Add(combo);
dailyMenu.Add(new ProductCatalogItem("Pizza", 18.00m, 0.6m));
```

### Principii SOLID aplicate
- **LSP**: ProductCatalogItem și ProductBundle sunt interschimbabile ca IProductCatalogComponent
- **OCP**: Noi tipuri de componente (ex. ProductDiscount) se adaugă fără a modifica cele existente
- **SRP**: Leaf gestionează un singur produs, composite doar agregarea copiilor

### API Endpoint
- `GET /api/catalog` — returnează structura ierarhică (Name, TotalPrice, TotalWeight, Children)

---

## 3. Façade Pattern

### Problemă rezolvată

Plasarea unei comenzi implică: validare client, procesare plată, creare comandă, salvare în repository, trimitere notificare. Un consumator (controller API) ar trebui să orchestreze manual toate aceste servicii, cunoscând ordinea și dependențele. Façade oferă o singură metodă `PlaceOrder` care ascunde complexitatea și expune un API simplu.

### Diagrama UML

```
@startuml
class OrderPlacementFacade {
  -_orderRepository: IOrderRepository
  -_customerRepository: ICustomerRepository
  -_gatewayProvider: IPaymentGatewayProvider
  -_notificationService: INotificationService
  +PlaceOrder(request): OrderPlacementResult
}

interface IOrderRepository
interface ICustomerRepository
interface IPaymentGatewayProvider
interface INotificationService

OrderPlacementFacade o--> IOrderRepository
OrderPlacementFacade o--> ICustomerRepository
OrderPlacementFacade o--> IPaymentGatewayProvider
OrderPlacementFacade o--> INotificationService

class OrdersController {
  +Place(request): ActionResult
}

OrdersController ..> OrderPlacementFacade : uses
@enduml
```

### Implementare

**OrderPlacementFacade** — interfață simplificată:

```csharp
public sealed class OrderPlacementFacade
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentGatewayProvider _gatewayProvider;
    private readonly INotificationService _notificationService;

    public OrderPlacementResult PlaceOrder(PlaceOrderRequest request)
    {
        var customer = _customerRepository.GetById(request.CustomerId);
        if (customer == null)
            return new OrderPlacementResult(null, null, false, "Customer not found.");

        var gateway = _gatewayProvider.GetGateway(request.PaymentGatewayType);
        var paymentResult = gateway.ProcessPayment(totalAmount, currency, referenceId);

        if (!paymentResult.Success)
            return new OrderPlacementResult(null, null, false, paymentResult.ErrorMessage);

        var order = new Order(request.CustomerId);
        foreach (var item in request.Items)
            order.AddItem(new OrderItem(...));

        _orderRepository.Add(order);
        _notificationService.NotifyOrderCreated(order, customer);

        return new OrderPlacementResult(order.Id, null, true, "Order placed successfully.");
    }
}
```

**Flux PlaceOrder:**
1. Validează existența clientului
2. Procesează plata prin IPaymentGateway (Adapter)
3. Dacă plata reușește: creează Order, salvează, trimite notificare
4. Returnează OrderPlacementResult (Success, OrderId, Message)

### Principii SOLID aplicate
- **SRP**: Façade orchestrează fluxul; serviciile individuale rămân independente
- **DIP**: Façade depinde de abstracții (IOrderRepository, IPaymentGatewayProvider etc.)
- **Simplificare**: Clientul (OrdersController) apelează o singură metodă în loc de 5–6 operații

### API Endpoint
- `POST /api/orders/place` — plasare comandă completă (CustomerId, Items, PaymentGateway, DeliveryNotes)

---

## Teste Unitare

### PaymentAdapterTests (8 teste)
- `PayPalAdapter_ProcessPayment_ReturnsSuccess` — Success=true, TransactionId cu prefix paypal_
- `PayPalAdapter_InvalidAmount_ReturnsFailure` — amount 0 returnează Failure
- `StripeAdapter_ProcessPayment_ReturnsSuccess` — Success, prefix ch_
- `StripeAdapter_InvalidKey_ReturnsFailure` — cheie goală returnează Failure
- `GooglePayAdapter_ProcessPayment_ReturnsSuccess` — Success, prefix gpay_
- `GooglePayAdapter_InvalidToken_ReturnsFailure` — token gol returnează Failure
- `AllAdapters_ImplementIPaymentGateway` — toate implementează interfața comună

### CompositeCatalogTests (8 teste)
- `ProductCatalogItem_ReturnsCorrectPrice` — preț și greutate corecte
- `ProductCatalogItem_GetChildren_ReturnsEmpty` — leaf returnează listă goală
- `ProductBundle_SumChildPrices` — sumă prețuri și greutăți copii
- `ProductBundle_NestedBundles_SumRecursively` — bundle de bundle-uri
- `ProductCatalogItem_ToOrderItems_ReturnsSingleItem` — conversie corectă
- `ProductBundle_ToOrderItems_FlattensAllChildren` — flatten recursiv
- `ProductCatalogItem_InvalidParams_Throws` — validare parametri
- `ProductBundle_ToOrderItems_WithQuantity_MultipliesCorrectly` — quantity aplicat

### OrderPlacementFacadeTests (5 teste)
- `PlaceOrder_Success_CreatesOrderAndReturnsSuccess` — order creat, salvat, notificat
- `PlaceOrder_PaymentFails_DoesNotCreateOrder` — la eșec plată, comandă nu se creează
- `PlaceOrder_CustomerNotFound_ReturnsFailure` — customer inexistent
- `PlaceOrder_EmptyItems_ReturnsFailure` — items goale
- `PlaceOrder_NullRequest_ThrowsArgumentNullException` — validare request

**Total Lab4: 21 teste — toate Passed**

---

## Structura Fișierelor Lab 4

```
DeliverySystem.Interfaces/
└── Payments/
    ├── IPaymentGateway.cs
    ├── IPaymentGatewayProvider.cs
    └── PaymentResult.cs

DeliverySystem.Domain/
└── Composite/
    ├── IProductCatalogComponent.cs
    ├── ProductCatalogItem.cs
    └── ProductBundle.cs

DeliverySystem.Infrastructure/
└── Payments/
    ├── ExternalApis/
    │   ├── PayPalApi.cs
    │   ├── StripeApi.cs
    │   └── GooglePayApi.cs
    ├── Adapters/
    │   ├── PayPalPaymentAdapter.cs
    │   ├── StripePaymentAdapter.cs
    │   └── GooglePayPaymentAdapter.cs
    └── PaymentGatewayProvider.cs

DeliverySystem.Services/
├── PaymentService.cs
├── OrderPlacementFacade.cs
└── CatalogProvider.cs

DeliverySystem.API/
├── Controllers/
│   ├── PaymentsController.cs
│   ├── CatalogController.cs
│   └── OrdersController.cs         (modificat: endpoint Place)
└── DTOs/
    ├── PaymentDtos.cs
    ├── CatalogDtos.cs
    └── OrderPlacementDtos.cs

DeliverySystem.Tests/
└── Lab4/
    ├── PaymentAdapterTests.cs
    ├── CompositeCatalogTests.cs
    ├── OrderPlacementFacadeTests.cs
    └── TestDoubles/
        ├── FailingPaymentGateway.cs
        ├── TestPaymentGatewayProvider.cs
        └── NullNotificationService.cs
```
