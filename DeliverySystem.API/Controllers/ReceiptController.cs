using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Templates;
using DeliverySystem.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/receipts")]
public class ReceiptController : ControllerBase
{
    private readonly IOrderRepository _orders;
    private readonly IDeliveryRepository _deliveries;
    private readonly ICustomerRepository _customers;
    private readonly ICourierRepository _couriers;

    public ReceiptController(
        IOrderRepository orders,
        IDeliveryRepository deliveries,
        ICustomerRepository customers,
        ICourierRepository couriers)
    {
        _orders = orders;
        _deliveries = deliveries;
        _customers = customers;
        _couriers = couriers;
    }

    [HttpPost]
    public ActionResult<ReceiptResponse> Generate([FromBody] ReceiptRequest request)
    {
        switch (request.Kind.ToLowerInvariant())
        {
            case "order":
            {
                var order = _orders.GetById(request.Id);
                if (order == null) return NotFound("Order not found.");

                var customer = _customers.GetById(order.CustomerId);
                var customerName = request.CustomerName ?? customer?.Name ?? "Unknown";
                var body = new OrderReceiptGenerator(order, customerName).Generate();
                return Ok(new ReceiptResponse("order", body));
            }
            case "delivery":
            {
                var delivery = _deliveries.GetById(request.Id);
                if (delivery == null) return NotFound("Delivery not found.");

                var courier = _couriers.GetById(delivery.CourierId);
                var courierName = request.CourierName ?? courier?.Name ?? "Unknown";
                var body = new DeliveryReceiptGenerator(delivery, courierName).Generate();
                return Ok(new ReceiptResponse("delivery", body));
            }
            case "refund":
            {
                if (!request.RefundAmount.HasValue || request.RefundAmount.Value <= 0)
                    return BadRequest("RefundAmount must be a positive number for refund receipts.");

                var order = _orders.GetById(request.Id);
                if (order == null) return NotFound("Order not found.");

                var body = new RefundReceiptGenerator(order, request.RefundAmount.Value, request.RefundReason ?? "n/a").Generate();
                return Ok(new ReceiptResponse("refund", body));
            }
            default:
                return BadRequest("Unknown receipt kind.");
        }
    }
}
