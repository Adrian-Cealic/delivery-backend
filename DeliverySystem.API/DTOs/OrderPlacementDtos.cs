namespace DeliverySystem.API.DTOs;

public record PlaceOrderItemDto(string ProductName, int Quantity, decimal UnitPrice, decimal Weight);

public record PlaceOrderRequestDto(
    Guid CustomerId,
    List<PlaceOrderItemDto> Items,
    string PaymentGateway,
    string? DeliveryNotes = null,
    string? Currency = null,
    string? Priority = null);

public record OrderPlacementResponseDto(
    Guid? OrderId,
    Guid? DeliveryId,
    bool Success,
    string Message);
