namespace DeliverySystem.Domain.States;

public sealed class InTransitDeliveryState : BaseDeliveryState
{
    public override string Name => "InTransit";

    public override void Complete(DeliveryStateContext context)
    {
        context.TransitionTo(new DeliveredDeliveryState());
    }
}
