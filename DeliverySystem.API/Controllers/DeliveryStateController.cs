using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.States;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/delivery-state")]
public class DeliveryStateController : ControllerBase
{
    private readonly DeliveryStateContext _context;

    public DeliveryStateController(DeliveryStateContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<StateSnapshotDto> Get() => Ok(Snapshot());

    [HttpPost("action")]
    public ActionResult<StateSnapshotDto> Apply([FromBody] StateActionRequest request)
    {
        try
        {
            switch (request.Action.ToLowerInvariant())
            {
                case "assign": _context.Assign(); break;
                case "pickup": _context.PickUp(); break;
                case "transit": _context.StartTransit(); break;
                case "complete": _context.Complete(); break;
                case "fail": _context.Fail(request.Reason ?? "n/a"); break;
                default: return BadRequest("Unknown action.");
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok(Snapshot());
    }

    private StateSnapshotDto Snapshot() =>
        new(_context.CurrentState, _context.IsTerminal, _context.FailureReason, _context.Trace);
}
