using DeliverySystem.API.DTOs;
using DeliverySystem.Interfaces.Payments;
using DeliverySystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliverySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
    }

    [HttpPost("process")]
    public ActionResult<ProcessPaymentResponse> Process([FromBody] ProcessPaymentRequest request)
    {
        if (!Enum.TryParse<PaymentGatewayType>(request.Gateway, true, out var gatewayType))
            return BadRequest(new { message = $"Invalid gateway: {request.Gateway}. Supported: PayPal, Stripe, GooglePay" });

        var referenceId = request.ReferenceId ?? Guid.NewGuid().ToString("N");
        var result = _paymentService.ProcessPayment(request.Amount, request.Currency, referenceId, gatewayType);

        return Ok(new ProcessPaymentResponse(result.Success, result.TransactionId, result.ErrorMessage));
    }
}
