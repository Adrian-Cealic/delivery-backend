namespace DeliverySystem.Domain.States;

/// <summary>
/// Context whose behaviour is delegated to its current IDeliveryState. Each transition
/// decision is made by the state itself, not by the context — true State pattern.
/// </summary>
public sealed class DeliveryStateContext
{
    private IDeliveryState _state;
    private readonly List<string> _trace = new();

    public DeliveryStateContext()
    {
        _state = new PendingDeliveryState();
        Record($"START in {_state.Name}");
    }

    public string CurrentState => _state.Name;
    public bool IsTerminal => _state.IsTerminal;
    public IReadOnlyList<string> Trace => _trace;
    public string? FailureReason { get; private set; }

    public void TransitionTo(IDeliveryState next)
    {
        Record($"{_state.Name} -> {next.Name}");
        _state = next;
    }

    public void RecordFailure(string reason)
    {
        FailureReason = reason;
        Record($"FAIL: {reason}");
    }

    public void Assign() => _state.Assign(this);
    public void PickUp() => _state.PickUp(this);
    public void StartTransit() => _state.StartTransit(this);
    public void Complete() => _state.Complete(this);
    public void Fail(string reason) => _state.Fail(this, reason);

    private void Record(string entry) => _trace.Add(entry);
}
