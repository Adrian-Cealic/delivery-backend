namespace DeliverySystem.API.DTOs;

public record ConfigResponse(
    string SystemName,
    decimal MaxDeliveryDistanceKm,
    string DefaultCurrency,
    int MaxOrderItems);
