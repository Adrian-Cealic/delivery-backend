using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.ValueObjects;
using DeliverySystem.Interfaces;
using DeliverySystem.Infrastructure.Repositories;
using DeliverySystem.Services;
using DeliverySystem.Tests.Lab4.TestDoubles;

namespace DeliverySystem.Tests.Lab4;

public class OrderPlacementFacadeTests
{
    private static (IOrderRepository, ICustomerRepository, Customer) CreateReposWithCustomer()
    {
        var orderRepo = new InMemoryOrderRepository();
        var customerRepo = new InMemoryCustomerRepository();
        var address = new Address("Street", "City", "12345", "Moldova");
        var customer = new Customer("Test User", "test@example.com", "+373-69-000000", address);
        customerRepo.Add(customer);
        return (orderRepo, customerRepo, customer);
    }

    [Fact]
    public void PlaceOrder_Success_CreatesOrderAndReturnsSuccess()
    {
        var (orderRepo, customerRepo, customer) = CreateReposWithCustomer();
        var gatewayProvider = new TestPaymentGatewayProvider(succeed: true);
        var notificationService = new NullNotificationService();
        var facade = new OrderPlacementFacade(orderRepo, customerRepo, gatewayProvider, notificationService);

        var request = new PlaceOrderRequest(
            customer.Id,
            [new PlaceOrderItem("Pizza", 2, 18m, 0.6m)],
            PaymentGatewayType.PayPal);

        var result = facade.PlaceOrder(request);

        Assert.True(result.Success);
        Assert.NotNull(result.OrderId);
        Assert.Contains("success", result.Message, StringComparison.OrdinalIgnoreCase);

        var order = orderRepo.GetById(result.OrderId!.Value);
        Assert.NotNull(order);
        Assert.Equal(customer.Id, order.CustomerId);
        Assert.Single(order.Items);
        Assert.Equal(36m, order.GetTotalPrice());
    }

    [Fact]
    public void PlaceOrder_PaymentFails_DoesNotCreateOrder()
    {
        var (orderRepo, customerRepo, customer) = CreateReposWithCustomer();
        var gatewayProvider = new TestPaymentGatewayProvider(succeed: false);
        var notificationService = new NullNotificationService();
        var facade = new OrderPlacementFacade(orderRepo, customerRepo, gatewayProvider, notificationService);

        var request = new PlaceOrderRequest(
            customer.Id,
            [new PlaceOrderItem("Coffee", 1, 3.50m, 0.15m)],
            PaymentGatewayType.Stripe);

        var result = facade.PlaceOrder(request);

        Assert.False(result.Success);
        Assert.Null(result.OrderId);
        Assert.Contains("Payment", result.Message, StringComparison.OrdinalIgnoreCase);

        var orders = orderRepo.GetAll().ToList();
        Assert.Empty(orders);
    }

    [Fact]
    public void PlaceOrder_CustomerNotFound_ReturnsFailure()
    {
        var orderRepo = new InMemoryOrderRepository();
        var customerRepo = new InMemoryCustomerRepository();
        var gatewayProvider = new TestPaymentGatewayProvider(succeed: true);
        var notificationService = new NullNotificationService();
        var facade = new OrderPlacementFacade(orderRepo, customerRepo, gatewayProvider, notificationService);

        var request = new PlaceOrderRequest(
            Guid.NewGuid(),
            [new PlaceOrderItem("Pizza", 1, 18m, 0.6m)],
            PaymentGatewayType.PayPal);

        var result = facade.PlaceOrder(request);

        Assert.False(result.Success);
        Assert.Null(result.OrderId);
        Assert.Contains("not found", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PlaceOrder_EmptyItems_ReturnsFailure()
    {
        var (orderRepo, customerRepo, customer) = CreateReposWithCustomer();
        var gatewayProvider = new TestPaymentGatewayProvider(succeed: true);
        var notificationService = new NullNotificationService();
        var facade = new OrderPlacementFacade(orderRepo, customerRepo, gatewayProvider, notificationService);

        var request = new PlaceOrderRequest(customer.Id, [], PaymentGatewayType.PayPal);

        var result = facade.PlaceOrder(request);

        Assert.False(result.Success);
        Assert.Null(result.OrderId);
    }

    [Fact]
    public void PlaceOrder_NullRequest_ThrowsArgumentNullException()
    {
        var (orderRepo, customerRepo, _) = CreateReposWithCustomer();
        var gatewayProvider = new TestPaymentGatewayProvider();
        var notificationService = new NullNotificationService();
        var facade = new OrderPlacementFacade(orderRepo, customerRepo, gatewayProvider, notificationService);

        Assert.Throws<ArgumentNullException>(() => facade.PlaceOrder(null!));
    }
}
