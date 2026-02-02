using DeliverySystem.Domain.Entities;
using DeliverySystem.Interfaces;

namespace DeliverySystem.Infrastructure.Repositories;

public class InMemoryCourierRepository : ICourierRepository
{
    private readonly Dictionary<Guid, Courier> _couriers = new();

    public Courier? GetById(Guid id)
    {
        _couriers.TryGetValue(id, out var courier);
        return courier;
    }

    public IEnumerable<Courier> GetAll()
    {
        return _couriers.Values.ToList();
    }

    public IEnumerable<Courier> GetAvailable()
    {
        return _couriers.Values.Where(c => c.IsAvailable).ToList();
    }

    public IEnumerable<Courier> GetAvailableForWeight(decimal weight)
    {
        return _couriers.Values
            .Where(c => c.IsAvailable && c.CanCarry(weight))
            .ToList();
    }

    public void Add(Courier courier)
    {
        if (courier == null)
            throw new ArgumentNullException(nameof(courier));

        if (_couriers.ContainsKey(courier.Id))
            throw new InvalidOperationException($"Courier with ID {courier.Id} already exists.");

        _couriers[courier.Id] = courier;
    }

    public void Update(Courier courier)
    {
        if (courier == null)
            throw new ArgumentNullException(nameof(courier));

        if (!_couriers.ContainsKey(courier.Id))
            throw new InvalidOperationException($"Courier with ID {courier.Id} not found.");

        _couriers[courier.Id] = courier;
    }

    public void Delete(Guid id)
    {
        if (!_couriers.ContainsKey(id))
            throw new InvalidOperationException($"Courier with ID {id} not found.");

        _couriers.Remove(id);
    }
}
