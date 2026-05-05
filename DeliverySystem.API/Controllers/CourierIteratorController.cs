using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Iterators;
using DeliverySystem.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/courier-iterator")]
public class CourierIteratorController : ControllerBase
{
    private readonly ICourierRepository _courierRepository;

    public CourierIteratorController(ICourierRepository courierRepository)
    {
        _courierRepository = courierRepository;
    }

    [HttpPost]
    public ActionResult<CourierIteratorResponse> Walk([FromBody] CourierIteratorRequest request)
    {
        var collection = new CourierCollection(_courierRepository.GetAll());
        var iter = ResolveIterator(collection, request);
        if (iter == null) return BadRequest("Unknown mode. Use insertion, available, vehicle, round-robin.");

        var names = new List<string>();
        while (iter.HasNext()) names.Add(iter.Next().Name);

        return Ok(new CourierIteratorResponse(request.Mode, names));
    }

    private static IDeliveryIterator<DeliverySystem.Domain.Entities.Courier>? ResolveIterator(
        CourierCollection collection,
        CourierIteratorRequest request)
    {
        return request.Mode.ToLowerInvariant() switch
        {
            "insertion" => collection.CreateInsertionOrderIterator(),
            "available" => collection.CreateAvailableIterator(),
            "vehicle" when Enum.TryParse<VehicleType>(request.VehicleType, ignoreCase: true, out var vt)
                => collection.CreateVehicleTypeIterator(vt),
            "round-robin" => collection.CreateRoundRobinIterator(request.RoundRobinSteps ?? collection.Count),
            _ => null
        };
    }
}
