namespace DeliverySystem.API.DTOs;

public record AssignDeliveryRequest(Guid OrderId, Guid CourierId, decimal DistanceKm);

public record DeliveryResponse(
    Guid Id,
    Guid OrderId,
    Guid CourierId,
    string Status,
    DateTime AssignedAt,
    DateTime? PickedUpAt,
    DateTime? DeliveredAt,
    decimal DistanceKm,
    string? EstimatedDeliveryTime);
