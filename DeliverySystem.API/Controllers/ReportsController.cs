using DeliverySystem.API.DTOs;
using DeliverySystem.Infrastructure.Reports;
using DeliverySystem.Interfaces;
using DeliverySystem.Interfaces.Reports;
using DeliverySystem.Services.Reports;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDeliveryRepository _deliveryRepository;

    public ReportsController(
        IOrderRepository orderRepository,
        IDeliveryRepository deliveryRepository)
    {
        _orderRepository = orderRepository;
        _deliveryRepository = deliveryRepository;
    }

    [HttpGet("orders")]
    public ActionResult<ReportResponse> GetOrderReport([FromQuery] string format = "console")
    {
        var renderer = ResolveRenderer(format);
        if (renderer == null)
            return BadRequest($"Unsupported format: {format}. Use 'console' or 'json'.");

        var report = new OrderSummaryReport(renderer, _orderRepository.GetAll());
        return Ok(new ReportResponse(format, report.Generate()));
    }

    [HttpGet("deliveries")]
    public ActionResult<ReportResponse> GetDeliveryReport([FromQuery] string format = "console")
    {
        var renderer = ResolveRenderer(format);
        if (renderer == null)
            return BadRequest($"Unsupported format: {format}. Use 'console' or 'json'.");

        var report = new DeliveryStatusReport(renderer, _deliveryRepository.GetAll());
        return Ok(new ReportResponse(format, report.Generate()));
    }

    private static IReportRenderer? ResolveRenderer(string format)
    {
        return format.ToLowerInvariant() switch
        {
            "console" => new ConsoleReportRenderer(),
            "json" => new JsonReportRenderer(),
            _ => null
        };
    }
}
