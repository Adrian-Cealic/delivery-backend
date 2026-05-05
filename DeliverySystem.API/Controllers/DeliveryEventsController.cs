using DeliverySystem.API.DTOs;
using DeliverySystem.Infrastructure.Observer;
using DeliverySystem.Interfaces;
using DeliverySystem.Interfaces.Observer;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/delivery-events")]
public class DeliveryEventsController : ControllerBase
{
    private readonly DeliveryStatusSubject _subject;
    private readonly DashboardDeliveryObserver _dashboard;
    private readonly IDeliveryRepository _deliveryRepository;

    public DeliveryEventsController(
        DeliveryStatusSubject subject,
        DashboardDeliveryObserver dashboard,
        IDeliveryRepository deliveryRepository)
    {
        _subject = subject;
        _dashboard = dashboard;
        _deliveryRepository = deliveryRepository;
    }

    [HttpGet("channels")]
    public ActionResult<IReadOnlyCollection<string>> GetChannels() => Ok(_subject.AttachedChannels);

    [HttpGet]
    public ActionResult<IReadOnlyList<DeliveryEventDto>> GetEvents()
    {
        var dto = _dashboard.Events
            .Select(e => new DeliveryEventDto(e.At, e.DeliveryId, e.From.ToString(), e.To.ToString()))
            .ToList();
        return Ok(dto);
    }

    [HttpPost("subscribe-email")]
    public ActionResult<string> SubscribeEmail([FromQuery] string email)
    {
        var observer = new EmailDeliveryObserver(email);
        _subject.Attach(observer);
        return Ok($"Subscribed: {observer.ChannelName}");
    }

    [HttpPost("subscribe-sms")]
    public ActionResult<string> SubscribeSms([FromQuery] string phone)
    {
        var observer = new SmsDeliveryObserver(phone);
        _subject.Attach(observer);
        return Ok($"Subscribed: {observer.ChannelName}");
    }

    [HttpPost("simulate-status")]
    public ActionResult<string> SimulateStatusChange([FromQuery] Guid deliveryId, [FromQuery] string action)
    {
        var delivery = _deliveryRepository.GetById(deliveryId);
        if (delivery == null) return NotFound($"Delivery {deliveryId} not found.");

        var previous = delivery.Status;
        try
        {
            switch (action.ToLowerInvariant())
            {
                case "assign": delivery.MarkAsAssigned(); break;
                case "pickup": delivery.MarkAsPickedUp(); break;
                case "transit": delivery.MarkAsInTransit(); break;
                case "deliver": delivery.MarkAsDelivered(); break;
                default: return BadRequest("Unknown action. Use assign, pickup, transit or deliver.");
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }

        _subject.Notify(delivery, previous);
        return Ok($"{previous} -> {delivery.Status}; observers notified.");
    }
}
