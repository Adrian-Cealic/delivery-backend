namespace DeliverySystem.Domain.States;

public sealed class FailedDeliveryState : BaseDeliveryState
{
    public override string Name => "Failed";
    public override bool IsTerminal => true;

    // Terminal — Fail no-ops; everything else inherits the rejecting base.
    public override void Fail(DeliveryStateContext context, string reason) { }
}
