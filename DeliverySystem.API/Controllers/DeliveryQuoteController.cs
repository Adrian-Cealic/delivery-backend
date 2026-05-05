using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Strategies;
using DeliverySystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/delivery-quote")]
public class DeliveryQuoteController : ControllerBase
{
    private static readonly Dictionary<string, Func<IDeliveryCostStrategy>> StrategyMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["standard"] = () => new StandardDeliveryCostStrategy(),
            ["express"] = () => new ExpressDeliveryCostStrategy(),
            ["economy"] = () => new EconomyDeliveryCostStrategy()
        };

    [HttpGet("strategies")]
    public ActionResult<IReadOnlyList<string>> GetStrategies()
    {
        return Ok(StrategyMap.Keys.OrderBy(k => k).ToArray());
    }

    [HttpPost]
    public ActionResult<DeliveryQuoteResponse> Quote([FromBody] DeliveryQuoteRequest request)
    {
        if (!StrategyMap.TryGetValue(request.Strategy, out var factory))
            return BadRequest($"Unknown strategy '{request.Strategy}'. Available: standard, express, economy.");

        var calculator = new DeliveryCostCalculator(factory());
        var quote = calculator.Quote(request.DistanceKm, request.WeightKg);

        return Ok(new DeliveryQuoteResponse(quote.Strategy, quote.Cost, quote.Eta.TotalMinutes));
    }

    [HttpPost("compare")]
    public ActionResult<IReadOnlyList<DeliveryQuoteResponse>> Compare([FromBody] DeliveryQuoteRequest request)
    {
        var responses = StrategyMap.Select(pair =>
        {
            var calc = new DeliveryCostCalculator(pair.Value());
            var quote = calc.Quote(request.DistanceKm, request.WeightKg);
            return new DeliveryQuoteResponse(quote.Strategy, quote.Cost, quote.Eta.TotalMinutes);
        }).ToList();

        return Ok(responses);
    }
}
