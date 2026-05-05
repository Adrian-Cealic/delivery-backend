using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Memento;
using DeliverySystem.Services.Memento;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/order-draft")]
public class OrderDraftController : ControllerBase
{
    private readonly OrderDraft _draft;
    private readonly OrderDraftCaretaker _caretaker;

    public OrderDraftController(OrderDraft draft, OrderDraftCaretaker caretaker)
    {
        _draft = draft;
        _caretaker = caretaker;
    }

    [HttpGet]
    public ActionResult<DraftStateDto> GetState() => Ok(MapState());

    [HttpPost("lines")]
    public ActionResult<DraftStateDto> AddLine([FromBody] AddDraftLineRequest request)
    {
        try
        {
            _draft.AddLine(request.ProductName, request.Quantity, request.UnitPrice, request.Weight);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok(MapState());
    }

    [HttpDelete("lines/{index}")]
    public ActionResult<DraftStateDto> RemoveLine(int index)
    {
        try
        {
            _draft.RemoveLineAt(index);
        }
        catch (ArgumentOutOfRangeException)
        {
            return NotFound($"Line index {index} is out of range.");
        }
        return Ok(MapState());
    }

    [HttpPost("priority/{priority}")]
    public ActionResult<DraftStateDto> SetPriority(string priority)
    {
        if (!Enum.TryParse<OrderPriority>(priority, ignoreCase: true, out var parsed))
            return BadRequest($"Unknown priority '{priority}'.");

        _draft.SetPriority(parsed);
        return Ok(MapState());
    }

    [HttpPost("notes")]
    public ActionResult<DraftStateDto> SetNotes([FromBody] string? notes)
    {
        _draft.SetDeliveryNotes(notes);
        return Ok(MapState());
    }

    [HttpPost("save")]
    public ActionResult<IReadOnlyList<DraftSnapshotDto>> Save([FromBody] SaveDraftRequest request)
    {
        try
        {
            _caretaker.Push(_draft.Save(request.Label));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok(_caretaker.Snapshots.Select(s => new DraftSnapshotDto(s.Label, s.SavedAt)).ToList());
    }

    [HttpGet("snapshots")]
    public ActionResult<IReadOnlyList<DraftSnapshotDto>> GetSnapshots()
        => Ok(_caretaker.Snapshots.Select(s => new DraftSnapshotDto(s.Label, s.SavedAt)).ToList());

    [HttpPost("restore")]
    public ActionResult<DraftStateDto> Restore([FromBody] RestoreDraftRequest request)
    {
        var memento = _caretaker.FindByLabel(request.Label);
        if (memento == null) return NotFound($"Snapshot '{request.Label}' not found.");

        _draft.Restore(memento);
        return Ok(MapState());
    }

    private DraftStateDto MapState()
    {
        var lines = _draft.Lines.Select(l => new DraftLineDto(l.ProductName, l.Quantity, l.UnitPrice, l.Weight)).ToList();
        return new DraftStateDto(lines, _draft.Priority.ToString(), _draft.DeliveryNotes, _draft.Total);
    }
}
