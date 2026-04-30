using DeliverySystem.Domain.States;

namespace DeliverySystem.Tests.Lab7;

public class StateTests
{
    [Fact]
    public void NewContext_StartsInPending()
    {
        var ctx = new DeliveryStateContext();

        Assert.Equal("Pending", ctx.CurrentState);
        Assert.False(ctx.IsTerminal);
    }

    [Fact]
    public void HappyPath_AllowsFullSequence()
    {
        var ctx = new DeliveryStateContext();
        ctx.Assign();
        ctx.PickUp();
        ctx.StartTransit();
        ctx.Complete();

        Assert.Equal("Delivered", ctx.CurrentState);
        Assert.True(ctx.IsTerminal);
    }

    [Fact]
    public void Pending_DoesNotAllowComplete()
    {
        var ctx = new DeliveryStateContext();

        Assert.Throws<InvalidOperationException>(() => ctx.Complete());
    }

    [Fact]
    public void Assigned_DoesNotAllowStartTransit()
    {
        var ctx = new DeliveryStateContext();
        ctx.Assign();

        Assert.Throws<InvalidOperationException>(() => ctx.StartTransit());
    }

    [Fact]
    public void Fail_FromAnyNonTerminal_TransitionsToFailed()
    {
        var ctx = new DeliveryStateContext();
        ctx.Assign();
        ctx.PickUp();
        ctx.Fail("courier accident");

        Assert.Equal("Failed", ctx.CurrentState);
        Assert.True(ctx.IsTerminal);
        Assert.Equal("courier accident", ctx.FailureReason);
    }

    [Fact]
    public void Delivered_RejectsFail()
    {
        var ctx = new DeliveryStateContext();
        ctx.Assign(); ctx.PickUp(); ctx.StartTransit(); ctx.Complete();

        Assert.Throws<InvalidOperationException>(() => ctx.Fail("nope"));
    }

    [Fact]
    public void Failed_State_RejectsForwardTransitions()
    {
        var ctx = new DeliveryStateContext();
        ctx.Fail("dispatcher cancelled");

        Assert.Throws<InvalidOperationException>(() => ctx.Assign());
    }

    [Fact]
    public void Trace_RecordsAllTransitions()
    {
        var ctx = new DeliveryStateContext();
        ctx.Assign();
        ctx.PickUp();

        Assert.Equal(3, ctx.Trace.Count);
        Assert.StartsWith("START", ctx.Trace[0]);
        Assert.Contains("Pending -> Assigned", ctx.Trace[1]);
        Assert.Contains("Assigned -> PickedUp", ctx.Trace[2]);
    }

    [Fact]
    public void TerminalState_CanBeQueriedWithoutTransition()
    {
        var ctx = new DeliveryStateContext();
        ctx.Assign(); ctx.PickUp(); ctx.StartTransit(); ctx.Complete();

        Assert.True(ctx.IsTerminal);
        Assert.Null(ctx.FailureReason);
    }
}
