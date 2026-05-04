using DeliverySystem.API.DTOs;
using DeliverySystem.Domain.Chain;
using DeliverySystem.Services.Chain;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/order-validation")]
public class OrderValidationController : ControllerBase
{
    [HttpPost]
    public ActionResult<OrderValidationResponse> Validate([FromBody] OrderValidationRequest request)
    {
        var pipeline = OrderValidationPipeline.Default(
            maxWeightKg: 25m,
            maxDistanceKm: 50m,
            allowedCountries: new[] { "RO", "MD" });

        var ctx = new OrderValidationContext
        {
            CustomerId = request.CustomerId,
            Lines = request.Lines.Select(l =>
                new OrderValidationLine(l.ProductName, l.Quantity, l.UnitPrice, l.Weight, l.InStock)).ToArray(),
            DistanceKm = request.DistanceKm,
            WalletBalance = request.WalletBalance,
            CustomerCountry = request.CustomerCountry
        };

        var result = pipeline.Validate(ctx);
        return Ok(new OrderValidationResponse(result.IsAccepted, result.Passes, result.Failures));
    }
}
