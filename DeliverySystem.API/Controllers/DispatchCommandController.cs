using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Commands;
using DeliverySystem.Interfaces;
using DeliverySystem.Services.Commands;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/dispatch")]
public class DispatchCommandController : ControllerBase
{
    private readonly DeliveryCommandInvoker _invoker;
    private readonly IDeliveryRepository _deliveryRepository;

    public DispatchCommandController(
        DeliveryCommandInvoker invoker,
        IDeliveryRepository deliveryRepository)
    {
        _invoker = invoker;
        _deliveryRepository = deliveryRepository;
    }

    [HttpPost("execute")]
    public ActionResult<CommandHistoryDto> Execute([FromBody] DispatchCommandRequest request)
    {
        var delivery = _deliveryRepository.GetById(request.DeliveryId);
        if (delivery == null) return NotFound($"Delivery {request.DeliveryId} not found.");

        IDeliveryCommand command = request.Action.ToLowerInvariant() switch
        {
            "assign" => new AssignCourierCommand(delivery),
            "pickup" => new PickUpDeliveryCommand(delivery),
            "transit" => new StartTransitCommand(delivery),
            "complete" => new CompleteDeliveryCommand(delivery),
            _ => null!
        };

        if (command == null)
            return BadRequest("Unknown action. Use assign, pickup, transit or complete.");

        try
        {
            _invoker.Execute(command);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok(new CommandHistoryDto(_invoker.History, _invoker.CanUndo, _invoker.CanRedo));
    }

    [HttpPost("undo")]
    public ActionResult<CommandHistoryDto> Undo()
    {
        if (!_invoker.CanUndo) return BadRequest("Nothing to undo.");
        _invoker.Undo();
        return Ok(new CommandHistoryDto(_invoker.History, _invoker.CanUndo, _invoker.CanRedo));
    }

    [HttpPost("redo")]
    public ActionResult<CommandHistoryDto> Redo()
    {
        if (!_invoker.CanRedo) return BadRequest("Nothing to redo.");
        _invoker.Redo();
        return Ok(new CommandHistoryDto(_invoker.History, _invoker.CanUndo, _invoker.CanRedo));
    }

    [HttpGet("history")]
    public ActionResult<CommandHistoryDto> GetHistory()
        => Ok(new CommandHistoryDto(_invoker.History, _invoker.CanUndo, _invoker.CanRedo));

    [HttpPost("clear")]
    public ActionResult Clear()
    {
        _invoker.Clear();
        return NoContent();
    }
}
