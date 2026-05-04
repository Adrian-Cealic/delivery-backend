using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Visitors;
using DeliverySystem.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/courier-visitor")]
public class CourierVisitorController : ControllerBase
{
    private readonly ICourierRepository _couriers;

    public CourierVisitorController(ICourierRepository couriers)
    {
        _couriers = couriers;
    }

    [HttpPost]
    public ActionResult<VisitorResponse> Run([FromBody] VisitorRequest request)
    {
        var couriers = _couriers.GetAll().ToList();

        var rows = request.Visitor.ToLowerInvariant() switch
        {
            "capacity" => RunVisitor(couriers, new CapacityReportVisitor()),
            "maintenance" => RunVisitor(couriers, new MaintenanceCostVisitor(), v => $"{v:F2} RON / month"),
            "eco" => RunVisitor(couriers, new EcoScoreVisitor(), v => $"{v}/100"),
            _ => null
        };

        if (rows == null) return BadRequest("Unknown visitor. Use capacity, maintenance, or eco.");

        return Ok(new VisitorResponse(request.Visitor, rows));
    }

    private static IReadOnlyList<VisitorRowDto> RunVisitor<TResult>(
        IEnumerable<Courier> couriers,
        ICourierVisitor<TResult> visitor,
        Func<TResult, string>? format = null)
    {
        return couriers.Select(c => new VisitorRowDto(
            c.Name,
            c.VehicleType.ToString(),
            format != null ? format(c.Accept(visitor)) : c.Accept(visitor)?.ToString() ?? string.Empty
        )).ToList();
    }
}
