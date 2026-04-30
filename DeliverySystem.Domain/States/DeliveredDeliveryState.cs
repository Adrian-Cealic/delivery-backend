namespace DeliverySystem.Domain.States;

public sealed class DeliveredDeliveryState : BaseDeliveryState
{
    public override string Name => "Delivered";
    public override bool IsTerminal => true;

    // Terminal — Fail not allowed once delivered.
    public override void Fail(DeliveryStateContext context, string reason)
        => throw new InvalidOperationException("Cannot mark a delivered shipment as failed.");
}
