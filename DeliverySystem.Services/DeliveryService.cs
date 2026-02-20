using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Infrastructure.Configuration;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Services;

public class DeliveryService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICourierRepository _courierRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INotificationService _notificationService;
    private readonly DeliverySystemConfiguration _config;

    public DeliveryService(
        IDeliveryRepository deliveryRepository,
        IOrderRepository orderRepository,
        ICourierRepository courierRepository,
        ICustomerRepository customerRepository,
        INotificationService notificationService,
        DeliverySystemConfiguration config)
    {
        _deliveryRepository = deliveryRepository ?? throw new ArgumentNullException(nameof(deliveryRepository));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _courierRepository = courierRepository ?? throw new ArgumentNullException(nameof(courierRepository));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public Delivery AssignCourierToOrder(Guid orderId, Guid courierId, decimal distanceKm)
    {
        if (distanceKm > _config.MaxDeliveryDistanceKm)
            throw new InvalidOperationException(
                $"Distance {distanceKm}km exceeds max allowed delivery distance of {_config.MaxDeliveryDistanceKm}km.");

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

    public Courier? FindAvailableCourier(decimal orderWeight)
    {
        var availableCouriers = _courierRepository.GetAvailableForWeight(orderWeight);
        return availableCouriers.FirstOrDefault();
    }

    public Delivery? GetDeliveryById(Guid deliveryId)
    {
        return _deliveryRepository.GetById(deliveryId);
    }

    public Delivery? GetDeliveryByOrderId(Guid orderId)
    {
        return _deliveryRepository.GetByOrderId(orderId);
    }

    public IEnumerable<Delivery> GetAllDeliveries()
    {
        return _deliveryRepository.GetAll();
    }

    public IEnumerable<Delivery> GetDeliveriesByCourier(Guid courierId)
    {
        return _deliveryRepository.GetByCourierId(courierId);
    }

    public void MarkPickedUp(Guid deliveryId)
    {
        var delivery = GetDeliveryOrThrow(deliveryId);
        var customer = GetCustomerForDelivery(delivery);

        delivery.MarkAsPickedUp();
        _deliveryRepository.Update(delivery);
        _notificationService.NotifyDeliveryStatusChanged(delivery, customer);
    }

    public void MarkInTransit(Guid deliveryId)
    {
        var delivery = GetDeliveryOrThrow(deliveryId);
        var customer = GetCustomerForDelivery(delivery);

        delivery.MarkAsInTransit();
        _deliveryRepository.Update(delivery);
        _notificationService.NotifyDeliveryStatusChanged(delivery, customer);
    }

    public void MarkDelivered(Guid deliveryId)
    {
        var delivery = GetDeliveryOrThrow(deliveryId);
        var customer = GetCustomerForDelivery(delivery);

        delivery.MarkAsDelivered();
        _deliveryRepository.Update(delivery);

        var courier = _courierRepository.GetById(delivery.CourierId);
        if (courier != null)
        {
            courier.SetAvailable();
            _courierRepository.Update(courier);
        }

        _notificationService.NotifyDeliveryCompleted(delivery, customer);
    }

    public void MarkFailed(Guid deliveryId)
    {
        var delivery = GetDeliveryOrThrow(deliveryId);
        var customer = GetCustomerForDelivery(delivery);

        delivery.MarkAsFailed();
        _deliveryRepository.Update(delivery);

        var courier = _courierRepository.GetById(delivery.CourierId);
        if (courier != null)
        {
            courier.SetAvailable();
            _courierRepository.Update(courier);
        }

        _notificationService.NotifyDeliveryStatusChanged(delivery, customer);
    }

    private Delivery GetDeliveryOrThrow(Guid deliveryId)
    {
        var delivery = _deliveryRepository.GetById(deliveryId);
        if (delivery == null)
            throw new InvalidOperationException($"Delivery with ID {deliveryId} not found.");

        return delivery;
    }

    private Customer GetCustomerForDelivery(Delivery delivery)
    {
        var order = _orderRepository.GetById(delivery.OrderId);
        if (order == null)
            throw new InvalidOperationException($"Order with ID {delivery.OrderId} not found.");

        var customer = _customerRepository.GetById(order.CustomerId);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {order.CustomerId} not found.");

        return customer;
    }
}
