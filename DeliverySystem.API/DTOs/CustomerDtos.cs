namespace DeliverySystem.API.DTOs;

public record AddressDto(string Street, string City, string PostalCode, string Country);

public record CreateCustomerRequest(string Name, string Email, string Phone, AddressDto Address);

public record UpdateCustomerRequest(string Name, string Email, string Phone, AddressDto Address);

public record CustomerResponse(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    AddressDto Address);
