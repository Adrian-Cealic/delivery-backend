using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Entities;
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

    [HttpPost("bike")]
    public ActionResult<CourierResponse> CreateBikeCourier([FromBody] CreateBikeCourierRequest request)
    {
        var courier = new BikeCourier(request.Name, request.Phone);
        _courierRepository.Add(courier);
        return CreatedAtAction(nameof(GetById), new { id = courier.Id }, MapToResponse(courier));
    }

    [HttpPost("car")]
    public ActionResult<CourierResponse> CreateCarCourier([FromBody] CreateCarCourierRequest request)
    {
        var courier = new CarCourier(request.Name, request.Phone, request.LicensePlate);
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
        if (courier is CarCourier carCourier)
            licensePlate = carCourier.LicensePlate;

        return new CourierResponse(
            courier.Id,
            courier.Name,
            courier.Phone,
            courier.IsAvailable,
            courier.VehicleType.ToString(),
            courier.MaxWeight,
            licensePlate);
    }
}
