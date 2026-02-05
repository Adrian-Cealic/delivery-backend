namespace DeliverySystem.API.DTOs;

public record OrderItemDto(string ProductName, int Quantity, decimal UnitPrice, decimal Weight);

public record CreateOrderRequest(Guid CustomerId, List<OrderItemDto> Items);

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    decimal TotalPrice,
    decimal TotalWeight,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<OrderItemDto> Items);
