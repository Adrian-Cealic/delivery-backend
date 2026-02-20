using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Services;

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

    public Order CreateOrder(Guid customerId, IEnumerable<OrderItem> items,
        OrderPriority priority = OrderPriority.Normal, string? deliveryNotes = null)
    {
        var customer = _customerRepository.GetById(customerId);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {customerId} not found.");

        var order = new Order(customerId);
        order.SetPriority(priority);

        if (deliveryNotes != null)
            order.SetDeliveryNotes(deliveryNotes);

        foreach (var item in items)
        {
            order.AddItem(item);
        }

        _orderRepository.Add(order);
        _notificationService.NotifyOrderCreated(order, customer);

        return order;
    }

    public Order RegisterOrder(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        var customer = _customerRepository.GetById(order.CustomerId);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {order.CustomerId} not found.");

        _orderRepository.Add(order);
        _notificationService.NotifyOrderCreated(order, customer);

        return order;
    }

    public Order CloneOrder(Guid orderId)
    {
        var original = GetOrderOrThrow(orderId);
        var customer = GetCustomerOrThrow(original.CustomerId);

        var clone = original.DeepCopy();

        _orderRepository.Add(clone);
        _notificationService.NotifyOrderCreated(clone, customer);

        return clone;
    }

    public Order? GetOrderById(Guid orderId)
    {
        return _orderRepository.GetById(orderId);
    }

    public IEnumerable<Order> GetAllOrders()
    {
        return _orderRepository.GetAll();
    }

    public IEnumerable<Order> GetOrdersByCustomer(Guid customerId)
    {
        return _orderRepository.GetByCustomerId(customerId);
    }

    public void ConfirmOrder(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        var customer = GetCustomerOrThrow(order.CustomerId);

        order.UpdateStatus(OrderStatus.Confirmed);
        _orderRepository.Update(order);
        _notificationService.NotifyOrderStatusChanged(order, customer);
    }

    public void StartProcessing(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        var customer = GetCustomerOrThrow(order.CustomerId);

        order.UpdateStatus(OrderStatus.Processing);
        _orderRepository.Update(order);
        _notificationService.NotifyOrderStatusChanged(order, customer);
    }

    public void MarkReadyForDelivery(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        var customer = GetCustomerOrThrow(order.CustomerId);

        order.UpdateStatus(OrderStatus.ReadyForDelivery);
        _orderRepository.Update(order);
        _notificationService.NotifyOrderStatusChanged(order, customer);
    }

    public void MarkInDelivery(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        var customer = GetCustomerOrThrow(order.CustomerId);

        order.UpdateStatus(OrderStatus.InDelivery);
        _orderRepository.Update(order);
        _notificationService.NotifyOrderStatusChanged(order, customer);
    }

    public void MarkDelivered(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        var customer = GetCustomerOrThrow(order.CustomerId);

        order.UpdateStatus(OrderStatus.Delivered);
        _orderRepository.Update(order);
        _notificationService.NotifyOrderStatusChanged(order, customer);
    }

    public void CancelOrder(Guid orderId)
    {
        var order = GetOrderOrThrow(orderId);
        var customer = GetCustomerOrThrow(order.CustomerId);

        order.UpdateStatus(OrderStatus.Cancelled);
        _orderRepository.Update(order);
        _notificationService.NotifyOrderStatusChanged(order, customer);
    }

    private Order GetOrderOrThrow(Guid orderId)
    {
        var order = _orderRepository.GetById(orderId);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {orderId} not found.");

        return order;
    }

    private Customer GetCustomerOrThrow(Guid customerId)
    {
        var customer = _customerRepository.GetById(customerId);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {customerId} not found.");

        return customer;
    }
}
