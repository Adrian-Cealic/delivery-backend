using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeliveriesController : ControllerBase
{
    private readonly DeliveryService _deliveryService;

    public DeliveriesController(DeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<DeliveryResponse>> GetAll()
    {
        var deliveries = _deliveryService.GetAllDeliveries();
        return Ok(deliveries.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<DeliveryResponse> GetById(Guid id)
    {
        var delivery = _deliveryService.GetDeliveryById(id);
        if (delivery == null)
            return NotFound(new { message = $"Delivery with ID {id} not found." });

        return Ok(MapToResponse(delivery));
    }

    [HttpGet("order/{orderId:guid}")]
    public ActionResult<DeliveryResponse> GetByOrder(Guid orderId)
    {
        var delivery = _deliveryService.GetDeliveryByOrderId(orderId);
        if (delivery == null)
            return NotFound(new { message = $"Delivery for order {orderId} not found." });

        return Ok(MapToResponse(delivery));
    }

    [HttpGet("courier/{courierId:guid}")]
    public ActionResult<IEnumerable<DeliveryResponse>> GetByCourier(Guid courierId)
    {
        var deliveries = _deliveryService.GetDeliveriesByCourier(courierId);
        return Ok(deliveries.Select(MapToResponse));
    }

    [HttpPost]
    public ActionResult<DeliveryResponse> Assign([FromBody] AssignDeliveryRequest request)
    {
        var delivery = _deliveryService.AssignCourierToOrder(
            request.OrderId, request.CourierId, request.DistanceKm);

        return CreatedAtAction(nameof(GetById), new { id = delivery.Id }, MapToResponse(delivery));
    }

    [HttpPost("{id:guid}/pickup")]
    public ActionResult<DeliveryResponse> MarkPickedUp(Guid id)
    {
        _deliveryService.MarkPickedUp(id);
        var delivery = _deliveryService.GetDeliveryById(id);
        return Ok(MapToResponse(delivery!));
    }

    [HttpPost("{id:guid}/transit")]
    public ActionResult<DeliveryResponse> MarkInTransit(Guid id)
    {
        _deliveryService.MarkInTransit(id);
        var delivery = _deliveryService.GetDeliveryById(id);
        return Ok(MapToResponse(delivery!));
    }

    [HttpPost("{id:guid}/deliver")]
    public ActionResult<DeliveryResponse> MarkDelivered(Guid id)
    {
        _deliveryService.MarkDelivered(id);
        var delivery = _deliveryService.GetDeliveryById(id);
        return Ok(MapToResponse(delivery!));
    }

    [HttpPost("{id:guid}/fail")]
    public ActionResult<DeliveryResponse> MarkFailed(Guid id)
    {
        _deliveryService.MarkFailed(id);
        var delivery = _deliveryService.GetDeliveryById(id);
        return Ok(MapToResponse(delivery!));
    }

    private static DeliveryResponse MapToResponse(Delivery delivery)
    {
        return new DeliveryResponse(
            delivery.Id,
            delivery.OrderId,
            delivery.CourierId,
            delivery.Status.ToString(),
            delivery.AssignedAt,
            delivery.PickedUpAt,
            delivery.DeliveredAt,
            delivery.DistanceKm,
            delivery.EstimatedDeliveryTime?.ToString());
    }
}
