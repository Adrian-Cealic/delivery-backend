using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Interfaces;
using DeliverySystem.Interfaces.Payments;

namespace DeliverySystem.Services;

public sealed class OrderPlacementFacade
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentGatewayProvider _gatewayProvider;
    private readonly INotificationService _notificationService;

    public OrderPlacementFacade(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IPaymentGatewayProvider gatewayProvider,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _gatewayProvider = gatewayProvider ?? throw new ArgumentNullException(nameof(gatewayProvider));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public OrderPlacementResult PlaceOrder(PlaceOrderRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var customer = _customerRepository.GetById(request.CustomerId);
        if (customer == null)
            return new OrderPlacementResult(null, null, false, $"Customer with ID {request.CustomerId} not found.");

        if (request.Items == null || !request.Items.Any())
            return new OrderPlacementResult(null, null, false, "Order must contain at least one item.");

        var totalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice);
        var referenceId = Guid.NewGuid().ToString("N");
        var gateway = _gatewayProvider.GetGateway(request.PaymentGatewayType);
        var paymentResult = gateway.ProcessPayment(totalAmount, request.Currency ?? "MDL", referenceId);

        if (!paymentResult.Success)
            return new OrderPlacementResult(null, null, false, paymentResult.ErrorMessage ?? "Payment failed.");

        var order = new Order(request.CustomerId);
        order.SetPriority(request.Priority);
        if (!string.IsNullOrWhiteSpace(request.DeliveryNotes))
            order.SetDeliveryNotes(request.DeliveryNotes);

        foreach (var item in request.Items)
            order.AddItem(new OrderItem(item.ProductName, item.Quantity, item.UnitPrice, item.Weight));

        _orderRepository.Add(order);
        _notificationService.NotifyOrderCreated(order, customer);

        return new OrderPlacementResult(order.Id, null, true, "Order placed successfully.");
    }
}

public record PlaceOrderItem(string ProductName, int Quantity, decimal UnitPrice, decimal Weight);

public record PlaceOrderRequest(
    Guid CustomerId,
    IReadOnlyList<PlaceOrderItem> Items,
    PaymentGatewayType PaymentGatewayType,
    string? DeliveryNotes = null,
    string? Currency = null,
    OrderPriority Priority = OrderPriority.Normal);

public record OrderPlacementResult(
    Guid? OrderId,
    Guid? DeliveryId,
    bool Success,
    string Message);
