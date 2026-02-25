namespace DeliverySystem.API.DTOs;

public record CatalogNodeDto(string Name, decimal TotalPrice, decimal TotalWeight, List<CatalogNodeDto> Children);
