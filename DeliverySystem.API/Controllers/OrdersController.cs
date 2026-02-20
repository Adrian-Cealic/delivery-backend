using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Builders;
using DeliverySystem.Domain.Entities;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<OrderResponse>> GetAll()
    {
        var orders = _orderService.GetAllOrders();
        return Ok(orders.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<OrderResponse> GetById(Guid id)
    {
        var order = _orderService.GetOrderById(id);
        if (order == null)
            return NotFound(new { message = $"Order with ID {id} not found." });

        return Ok(MapToResponse(order));
    }

    [HttpGet("customer/{customerId:guid}")]
    public ActionResult<IEnumerable<OrderResponse>> GetByCustomer(Guid customerId)
    {
        var orders = _orderService.GetOrdersByCustomer(customerId);
        return Ok(orders.Select(MapToResponse));
    }

    [HttpPost]
    public ActionResult<OrderResponse> Create([FromBody] CreateOrderRequest request)
    {
        var items = request.Items.Select(i =>
            new OrderItem(i.ProductName, i.Quantity, i.UnitPrice, i.Weight)).ToList();

        var priority = OrderPriority.Normal;
        if (!string.IsNullOrWhiteSpace(request.Priority))
        {
            if (!Enum.TryParse<OrderPriority>(request.Priority, true, out priority))
                return BadRequest(new { message = $"Invalid priority: {request.Priority}. Supported: Economy, Normal, Express" });
        }

        var order = _orderService.CreateOrder(request.CustomerId, items, priority, request.DeliveryNotes);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapToResponse(order));
    }

    [HttpPost("builder")]
    public ActionResult<OrderResponse> CreateWithBuilder([FromBody] CreateOrderWithBuilderRequest request)
    {
        if (!Enum.TryParse<OrderPriority>(request.Priority, true, out var priority))
            return BadRequest(new { message = $"Invalid priority: {request.Priority}. Supported: Economy, Normal, Express" });

        var builder = new StandardOrderBuilder();
        builder.SetCustomerId(request.CustomerId)
            .SetPriority(priority)
            .SetDeliveryNotes(request.DeliveryNotes);

        foreach (var item in request.Items)
            builder.AddItem(new OrderItem(item.ProductName, item.Quantity, item.UnitPrice, item.Weight));

        var order = builder.Build();
        var savedOrder = _orderService.RegisterOrder(order);
        return CreatedAtAction(nameof(GetById), new { id = savedOrder.Id }, MapToResponse(savedOrder));
    }

    [HttpPost("{id:guid}/clone")]
    public ActionResult<OrderResponse> Clone(Guid id)
    {
        var clone = _orderService.CloneOrder(id);
        return CreatedAtAction(nameof(GetById), new { id = clone.Id }, MapToResponse(clone));
    }

    [HttpPost("{id:guid}/confirm")]
    public ActionResult<OrderResponse> Confirm(Guid id)
    {
        _orderService.ConfirmOrder(id);
        var order = _orderService.GetOrderById(id);
        return Ok(MapToResponse(order!));
    }

    [HttpPost("{id:guid}/process")]
    public ActionResult<OrderResponse> Process(Guid id)
    {
        _orderService.StartProcessing(id);
        var order = _orderService.GetOrderById(id);
        return Ok(MapToResponse(order!));
    }

    [HttpPost("{id:guid}/ready")]
    public ActionResult<OrderResponse> MarkReady(Guid id)
    {
        _orderService.MarkReadyForDelivery(id);
        var order = _orderService.GetOrderById(id);
        return Ok(MapToResponse(order!));
    }

    [HttpPost("{id:guid}/cancel")]
    public ActionResult<OrderResponse> Cancel(Guid id)
    {
        _orderService.CancelOrder(id);
        var order = _orderService.GetOrderById(id);
        return Ok(MapToResponse(order!));
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.Priority.ToString(),
            order.GetTotalPrice(),
            order.GetTotalWeight(),
            order.DeliveryNotes,
            order.CreatedAt,
            order.UpdatedAt,
            order.Items.Select(i => new OrderItemDto(i.ProductName, i.Quantity, i.UnitPrice, i.Weight)).ToList());
    }
}
