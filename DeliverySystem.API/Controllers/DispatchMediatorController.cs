using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Mediator;
using DeliverySystem.Services.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/mediator")]
public class DispatchMediatorController : ControllerBase
{
    private readonly DispatchMediator _mediator;
    private readonly NotifierDispatchParticipant _notifier;

    public DispatchMediatorController(DispatchMediator mediator, NotifierDispatchParticipant notifier)
    {
        _mediator = mediator;
        _notifier = notifier;
    }

    [HttpPost("register")]
    public ActionResult<MediatorBroadcastDto> Register([FromBody] MediatorParticipantDto request)
    {
        IDispatchParticipant participant = request.Kind.ToLowerInvariant() switch
        {
            "order" => new OrderDispatchParticipant(request.Name),
            "courier" => new CourierDispatchParticipant(request.Name),
            _ => throw new ArgumentException($"Unknown kind '{request.Kind}'.")
        };

        _mediator.Register(participant);
        return Ok(Snapshot());
    }

    [HttpPost("dispatch")]
    public ActionResult<MediatorBroadcastDto> Dispatch([FromQuery] string orderId)
    {
        var key = $"Order:{orderId}";
        if (_mediator.RegisteredIds.FirstOrDefault(id => id == key) is null)
            return NotFound($"Order participant '{key}' not registered.");

        // Reconstruct via the registered participant by sending through the mediator
        var dummy = new OrderDispatchParticipant(orderId);
        _mediator.Send(dummy, new DispatchMessage("order.ready", $"Order {key} ready for courier assignment."));
        return Ok(Snapshot());
    }

    [HttpGet]
    public ActionResult<MediatorBroadcastDto> Get() => Ok(Snapshot());

    private MediatorBroadcastDto Snapshot()
        => new(_mediator.RegisteredIds.ToList(), _mediator.Log, _notifier.Emitted);
}
