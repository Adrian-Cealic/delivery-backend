using DeliverySystem.API.DTOs;
using DeliverySystem.Infrastructure.Repositories.Proxies;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/orders/protected")]
public class ProtectedOrdersController : ControllerBase
{
    private readonly ProtectionOrderRepositoryProxy _proxy;

    public ProtectedOrdersController(ProtectionOrderRepositoryProxy proxy)
    {
        _proxy = proxy;
    }

    [HttpGet]
    public ActionResult<IEnumerable<OrderResponse>> GetAll()
    {
        try
        {
            var orders = _proxy.GetAll().Select(MapToResponse);
            return Ok(orders);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Error = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public ActionResult<OrderResponse> GetById(Guid id)
    {
        try
        {
            var order = _proxy.GetById(id);
            if (order == null)
                return NotFound();

            return Ok(MapToResponse(order));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Error = ex.Message });
        }
    }

    private static OrderResponse MapToResponse(Domain.Entities.Order o) => new(
        o.Id,
        o.CustomerId,
        o.Status.ToString(),
        o.Priority.ToString(),
        o.GetTotalPrice(),
        o.GetTotalWeight(),
        o.DeliveryNotes,
        o.CreatedAt,
        o.UpdatedAt,
        o.Items.Select(i => new OrderItemDto(i.ProductName, i.Quantity, i.UnitPrice, i.Weight)).ToList()
    );
}
