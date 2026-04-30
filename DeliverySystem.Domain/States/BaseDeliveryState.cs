namespace DeliverySystem.Domain.States;

/// <summary>
/// Default base state: every operation throws unless the concrete state overrides it.
/// Forces each state class to declare exactly what it allows.
/// </summary>
public abstract class BaseDeliveryState : IDeliveryState
{
    public abstract string Name { get; }
    public virtual bool IsTerminal => false;

    public virtual void Assign(DeliveryStateContext context) => Reject(nameof(Assign));
    public virtual void PickUp(DeliveryStateContext context) => Reject(nameof(PickUp));
    public virtual void StartTransit(DeliveryStateContext context) => Reject(nameof(StartTransit));
    public virtual void Complete(DeliveryStateContext context) => Reject(nameof(Complete));

    public virtual void Fail(DeliveryStateContext context, string reason)
    {
        context.RecordFailure(reason);
        context.TransitionTo(new FailedDeliveryState());
    }

    private void Reject(string action)
        => throw new InvalidOperationException($"Action '{action}' not allowed in state '{Name}'.");
}
