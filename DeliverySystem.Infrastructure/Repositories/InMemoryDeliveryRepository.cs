using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Infrastructure.Repositories;

public class InMemoryDeliveryRepository : IDeliveryRepository
{
    private readonly Dictionary<Guid, Delivery> _deliveries = new();

    public Delivery? GetById(Guid id)
    {
        _deliveries.TryGetValue(id, out var delivery);
        return delivery;
    }

    public Delivery? GetByOrderId(Guid orderId)
    {
        return _deliveries.Values.FirstOrDefault(d => d.OrderId == orderId);
    }

    public IEnumerable<Delivery> GetAll()
    {
        return _deliveries.Values.ToList();
    }

    public IEnumerable<Delivery> GetByCourierId(Guid courierId)
    {
        return _deliveries.Values.Where(d => d.CourierId == courierId).ToList();
    }

    public void Add(Delivery delivery)
    {
        if (delivery == null)
            throw new ArgumentNullException(nameof(delivery));

        if (_deliveries.ContainsKey(delivery.Id))
            throw new InvalidOperationException($"Delivery with ID {delivery.Id} already exists.");

        _deliveries[delivery.Id] = delivery;
    }

    public void Update(Delivery delivery)
    {
        if (delivery == null)
            throw new ArgumentNullException(nameof(delivery));

        if (!_deliveries.ContainsKey(delivery.Id))
            throw new InvalidOperationException($"Delivery with ID {delivery.Id} not found.");

        _deliveries[delivery.Id] = delivery;
    }

    public void Delete(Guid id)
    {
        if (!_deliveries.ContainsKey(id))
            throw new InvalidOperationException($"Delivery with ID {id} not found.");

        _deliveries.Remove(id);
    }
}
