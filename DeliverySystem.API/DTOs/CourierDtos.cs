namespace DeliverySystem.API.DTOs;

public record CreateCourierRequest(
    string VehicleType,
    string Name,
    string Phone,
    string? LicensePlate = null,
    decimal? MaxFlightRangeKm = null);

public record CreateBikeCourierRequest(string Name, string Phone);

public record CreateCarCourierRequest(string Name, string Phone, string LicensePlate);

public record CreateDroneCourierRequest(string Name, string Phone, decimal MaxFlightRangeKm);

public record CourierResponse(
    Guid Id,
    string Name,
    string Phone,
    bool IsAvailable,
    string VehicleType,
    decimal MaxWeight,
    string? LicensePlate,
    decimal? MaxFlightRangeKm = null);
