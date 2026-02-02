using DeliverySystem.Domain.Enums;

namespace DeliverySystem.Domain.Entities;

public class Delivery : EntityBase
{
    public Guid OrderId { get; private set; }
    public Guid CourierId { get; private set; }
    public DeliveryStatus Status { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? PickedUpAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public decimal DistanceKm { get; private set; }
    public TimeSpan? EstimatedDeliveryTime { get; private set; }

    public Delivery(Guid orderId, Guid courierId, decimal distanceKm)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));

        if (courierId == Guid.Empty)
            throw new ArgumentException("Courier ID cannot be empty.", nameof(courierId));

        if (distanceKm < 0)
            throw new ArgumentException("Distance cannot be negative.", nameof(distanceKm));

        OrderId = orderId;
        CourierId = courierId;
        DistanceKm = distanceKm;
        Status = DeliveryStatus.Pending;
        AssignedAt = DateTime.UtcNow;
    }

    public Delivery(Guid id, Guid orderId, Guid courierId, decimal distanceKm) : base(id)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty.", nameof(orderId));

        if (courierId == Guid.Empty)
            throw new ArgumentException("Courier ID cannot be empty.", nameof(courierId));

        if (distanceKm < 0)
            throw new ArgumentException("Distance cannot be negative.", nameof(distanceKm));

        OrderId = orderId;
        CourierId = courierId;
        DistanceKm = distanceKm;
        Status = DeliveryStatus.Pending;
        AssignedAt = DateTime.UtcNow;
    }

    public void SetEstimatedDeliveryTime(TimeSpan estimatedTime)
    {
        EstimatedDeliveryTime = estimatedTime;
    }

    public void MarkAsAssigned()
    {
        if (Status != DeliveryStatus.Pending)
            throw new InvalidOperationException($"Cannot mark as assigned. Current status: {Status}");

        Status = DeliveryStatus.Assigned;
    }

    public void MarkAsPickedUp()
    {
        if (Status != DeliveryStatus.Assigned)
            throw new InvalidOperationException($"Cannot mark as picked up. Current status: {Status}");

        Status = DeliveryStatus.PickedUp;
        PickedUpAt = DateTime.UtcNow;
    }

    public void MarkAsInTransit()
    {
        if (Status != DeliveryStatus.PickedUp)
            throw new InvalidOperationException($"Cannot mark as in transit. Current status: {Status}");

        Status = DeliveryStatus.InTransit;
    }

    public void MarkAsDelivered()
    {
        if (Status != DeliveryStatus.InTransit)
            throw new InvalidOperationException($"Cannot mark as delivered. Current status: {Status}");

        Status = DeliveryStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        if (Status == DeliveryStatus.Delivered)
            throw new InvalidOperationException("Cannot mark as failed. Delivery is already completed.");

        Status = DeliveryStatus.Failed;
    }

    public override string ToString()
    {
        return $"Delivery {Id}: Order {OrderId}, Courier {CourierId}, Status: {Status}";
    }
}
