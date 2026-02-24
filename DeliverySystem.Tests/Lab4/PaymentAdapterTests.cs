using DeliverySystem.Infrastructure.Payments.Adapters;
using DeliverySystem.Infrastructure.Payments.ExternalApis;
using DeliverySystem.Interfaces.Payments;

namespace DeliverySystem.Tests.Lab4;

public class PaymentAdapterTests
{
    [Fact]
    public void PayPalAdapter_ProcessPayment_ReturnsSuccess()
    {
        var api = new PayPalApi();
        var adapter = new PayPalPaymentAdapter(api);
        var result = adapter.ProcessPayment(100m, "MDL", "ref-001");

        Assert.True(result.Success);
        Assert.NotNull(result.TransactionId);
        Assert.StartsWith("paypal_", result.TransactionId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void PayPalAdapter_InvalidAmount_ReturnsFailure()
    {
        var api = new PayPalApi();
        var adapter = new PayPalPaymentAdapter(api);
        var result = adapter.ProcessPayment(0m, "MDL", "ref-001");

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("positive", result.ErrorMessage);
    }

    [Fact]
    public void StripeAdapter_ProcessPayment_ReturnsSuccess()
    {
        var api = new StripeApi();
        var adapter = new StripePaymentAdapter(api);
        var result = adapter.ProcessPayment(50.50m, "USD", "ref-002");

        Assert.True(result.Success);
        Assert.NotNull(result.TransactionId);
        Assert.StartsWith("ch_", result.TransactionId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void StripeAdapter_InvalidKey_ReturnsFailure()
    {
        var result = new StripeApi().Charge("", 1000, "key-001");

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public void GooglePayAdapter_ProcessPayment_ReturnsSuccess()
    {
        var api = new GooglePayApi();
        var adapter = new GooglePayPaymentAdapter(api);
        var result = adapter.ProcessPayment(25.00m, "EUR", "ref-003");

        Assert.True(result.Success);
        Assert.NotNull(result.TransactionId);
        Assert.StartsWith("gpay_", result.TransactionId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void GooglePayAdapter_InvalidToken_ReturnsFailure()
    {
        var api = new GooglePayApi();
        var request = new GooglePayPaymentRequest(10m, "MDL", "ref");
        var result = api.ProcessPayment("", request);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public void AllAdapters_ImplementIPaymentGateway()
    {
        IPaymentGateway paypal = new PayPalPaymentAdapter(new PayPalApi());
        IPaymentGateway stripe = new StripePaymentAdapter(new StripeApi());
        IPaymentGateway googlePay = new GooglePayPaymentAdapter(new GooglePayApi());

        Assert.True(paypal.ProcessPayment(1m, "MDL", "r1").Success);
        Assert.True(stripe.ProcessPayment(1m, "MDL", "r2").Success);
        Assert.True(googlePay.ProcessPayment(1m, "MDL", "r3").Success);
    }
}
