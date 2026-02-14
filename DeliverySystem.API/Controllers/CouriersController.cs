using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Factories;
using DeliverySystem.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouriersController : ControllerBase
{
    private readonly ICourierRepository _courierRepository;

    public CouriersController(ICourierRepository courierRepository)
    {
        _courierRepository = courierRepository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CourierResponse>> GetAll()
    {
        var couriers = _courierRepository.GetAll();
        return Ok(couriers.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<CourierResponse> GetById(Guid id)
    {
        var courier = _courierRepository.GetById(id);
        if (courier == null)
            return NotFound(new { message = $"Courier with ID {id} not found." });

        return Ok(MapToResponse(courier));
    }

    [HttpGet("available")]
    public ActionResult<IEnumerable<CourierResponse>> GetAvailable()
    {
        var couriers = _courierRepository.GetAvailable();
        return Ok(couriers.Select(MapToResponse));
    }

    [HttpGet("available/{weight:decimal}")]
    public ActionResult<IEnumerable<CourierResponse>> GetAvailableForWeight(decimal weight)
    {
        var couriers = _courierRepository.GetAvailableForWeight(weight);
        return Ok(couriers.Select(MapToResponse));
    }

    [HttpPost]
    public ActionResult<CourierResponse> CreateCourier([FromBody] CreateCourierRequest request)
    {
        if (!Enum.TryParse<VehicleType>(request.VehicleType, true, out var vehicleType))
            return BadRequest(new { message = $"Invalid vehicle type: {request.VehicleType}. Supported: Bicycle, Car, Drone" });

        var factory = CourierFactoryProvider.GetFactory(vehicleType);
        var parameters = new CourierCreationParams(
            request.Name,
            request.Phone,
            request.LicensePlate,
            request.MaxFlightRangeKm);

        var courier = factory.CreateAndValidate(parameters);
        _courierRepository.Add(courier);

        return CreatedAtAction(nameof(GetById), new { id = courier.Id }, MapToResponse(courier));
    }

    [HttpPost("bike")]
    public ActionResult<CourierResponse> CreateBikeCourier([FromBody] CreateBikeCourierRequest request)
    {
        var factory = CourierFactoryProvider.GetFactory(VehicleType.Bicycle);
        var courier = factory.CreateAndValidate(new CourierCreationParams(request.Name, request.Phone));
        _courierRepository.Add(courier);
        return CreatedAtAction(nameof(GetById), new { id = courier.Id }, MapToResponse(courier));
    }

    [HttpPost("car")]
    public ActionResult<CourierResponse> CreateCarCourier([FromBody] CreateCarCourierRequest request)
    {
        var factory = CourierFactoryProvider.GetFactory(VehicleType.Car);
        var courier = factory.CreateAndValidate(
            new CourierCreationParams(request.Name, request.Phone, request.LicensePlate));
        _courierRepository.Add(courier);
        return CreatedAtAction(nameof(GetById), new { id = courier.Id }, MapToResponse(courier));
    }

    [HttpPost("drone")]
    public ActionResult<CourierResponse> CreateDroneCourier([FromBody] CreateDroneCourierRequest request)
    {
        var factory = CourierFactoryProvider.GetFactory(VehicleType.Drone);
        var courier = factory.CreateAndValidate(
            new CourierCreationParams(request.Name, request.Phone, maxFlightRangeKm: request.MaxFlightRangeKm));
        _courierRepository.Add(courier);
        return CreatedAtAction(nameof(GetById), new { id = courier.Id }, MapToResponse(courier));
    }

    [HttpDelete("{id:guid}")]
    public ActionResult Delete(Guid id)
    {
        var courier = _courierRepository.GetById(id);
        if (courier == null)
            return NotFound(new { message = $"Courier with ID {id} not found." });

        _courierRepository.Delete(id);
        return NoContent();
    }

    private static CourierResponse MapToResponse(Courier courier)
    {
        string? licensePlate = null;
        decimal? maxFlightRange = null;

        if (courier is CarCourier carCourier)
            licensePlate = carCourier.LicensePlate;

        if (courier is DroneCourier droneCourier)
            maxFlightRange = droneCourier.MaxFlightRangeKm;

        return new CourierResponse(
            courier.Id,
            courier.Name,
            courier.Phone,
            courier.IsAvailable,
            courier.VehicleType.ToString(),
            courier.MaxWeight,
            licensePlate,
            maxFlightRange);
    }
}
