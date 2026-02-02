using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Infrastructure.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = new();

    public Order? GetById(Guid id)
    {
        _orders.TryGetValue(id, out var order);
        return order;
    }

    public IEnumerable<Order> GetAll()
    {
        return _orders.Values.ToList();
    }

    public IEnumerable<Order> GetByCustomerId(Guid customerId)
    {
        return _orders.Values.Where(o => o.CustomerId == customerId).ToList();
    }

    public void Add(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (_orders.ContainsKey(order.Id))
            throw new InvalidOperationException($"Order with ID {order.Id} already exists.");

        _orders[order.Id] = order;
    }

    public void Update(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (!_orders.ContainsKey(order.Id))
            throw new InvalidOperationException($"Order with ID {order.Id} not found.");

        _orders[order.Id] = order;
    }

    public void Delete(Guid id)
    {
        if (!_orders.ContainsKey(id))
            throw new InvalidOperationException($"Order with ID {id} not found.");

        _orders.Remove(id);
    }
}
