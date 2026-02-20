namespace DeliverySystem.API.DTOs;

public record OrderItemDto(string ProductName, int Quantity, decimal UnitPrice, decimal Weight);

public record CreateOrderRequest(
    Guid CustomerId,
    List<OrderItemDto> Items,
    string? Priority = null,
    string? DeliveryNotes = null);

public record CreateOrderWithBuilderRequest(
    Guid CustomerId,
    List<OrderItemDto> Items,
    string Priority,
    string? DeliveryNotes = null);

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    string Priority,
    decimal TotalPrice,
    decimal TotalWeight,
    string? DeliveryNotes,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<OrderItemDto> Items);
