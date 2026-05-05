using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Iterators;

/// <summary>
/// Aggregate exposing several traversal strategies for couriers without revealing the
/// underlying storage. Each Create* method returns a fresh iterator.
/// </summary>
public sealed class CourierCollection
{
    private readonly List<Courier> _couriers;

    public CourierCollection(IEnumerable<Courier> couriers)
    {
        if (couriers == null) throw new ArgumentNullException(nameof(couriers));
        _couriers = couriers.ToList();
    }

    public int Count => _couriers.Count;

    public IDeliveryIterator<Courier> CreateInsertionOrderIterator()
        => new InsertionOrderCourierIterator(_couriers);

    public IDeliveryIterator<Courier> CreateAvailableIterator()
        => new AvailableCourierIterator(_couriers);

    public IDeliveryIterator<Courier> CreateVehicleTypeIterator(VehicleType vehicleType)
        => new VehicleTypeCourierIterator(_couriers, vehicleType);

    public IDeliveryIterator<Courier> CreateRoundRobinIterator(int totalSteps)
        => new RoundRobinCourierIterator(_couriers, totalSteps);
}
