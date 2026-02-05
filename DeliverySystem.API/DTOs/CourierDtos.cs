namespace DeliverySystem.API.DTOs;

public record CreateBikeCourierRequest(string Name, string Phone);

public record CreateCarCourierRequest(string Name, string Phone, string LicensePlate);

public record CourierResponse(
    Guid Id,
    string Name,
    string Phone,
    bool IsAvailable,
    string VehicleType,
    decimal MaxWeight,
    string? LicensePlate);
