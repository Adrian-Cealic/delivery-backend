using DeliverySystem.Domain.Enums;
using DeliverySystem.Domain.Memento;
using DeliverySystem.Services.Memento;

namespace DeliverySystem.Tests.Lab6;

public class MementoTests
{
    [Fact]
    public void Save_CapturesCurrentLines()
    {
        var draft = new OrderDraft();
        draft.AddLine("Pizza", 2, 15m, 0.5m);
        draft.AddLine("Cola", 1, 5m, 0.3m);

        var memento = draft.Save("v1");

        Assert.Equal("v1", memento.Label);
    }

    [Fact]
    public void Restore_BringsBackPreviousState()
    {
        var draft = new OrderDraft();
        draft.AddLine("Pizza", 2, 15m, 0.5m);
        draft.SetPriority(OrderPriority.Express);
        draft.SetDeliveryNotes("Ring twice");

        var snapshot = draft.Save("v1");

        draft.AddLine("Cola", 1, 5m, 0.3m);
        draft.SetPriority(OrderPriority.Normal);
        draft.SetDeliveryNotes(null);
        Assert.Equal(2, draft.Lines.Count);

        draft.Restore(snapshot);

        Assert.Single(draft.Lines);
        Assert.Equal("Pizza", draft.Lines[0].ProductName);
        Assert.Equal(OrderPriority.Express, draft.Priority);
        Assert.Equal("Ring twice", draft.DeliveryNotes);
    }

    [Fact]
    public void Memento_IsImmutable_DraftMutationsDontLeak()
    {
        var draft = new OrderDraft();
        draft.AddLine("Pizza", 2, 15m, 0.5m);
        var snapshot = draft.Save("v1");

        draft.AddLine("Burger", 1, 10m, 0.4m);
        draft.Restore(snapshot);

        Assert.Single(draft.Lines);
        Assert.Equal("Pizza", draft.Lines[0].ProductName);
    }

    [Fact]
    public void Caretaker_StoresAndFindsSnapshotsByLabel()
    {
        var draft = new OrderDraft();
        var caretaker = new OrderDraftCaretaker();

        draft.AddLine("Pizza", 1, 15m, 0.5m);
        caretaker.Push(draft.Save("after-pizza"));

        draft.AddLine("Cola", 2, 5m, 0.3m);
        caretaker.Push(draft.Save("after-cola"));

        var found = caretaker.FindByLabel("after-pizza");

        Assert.NotNull(found);
        Assert.Equal("after-pizza", found!.Label);
        Assert.Equal(2, caretaker.Count);
    }

    [Fact]
    public void Caretaker_PopLast_ReturnsAndRemoves()
    {
        var draft = new OrderDraft();
        var caretaker = new OrderDraftCaretaker();
        draft.AddLine("Pizza", 1, 15m, 0.5m);
        caretaker.Push(draft.Save("v1"));
        caretaker.Push(draft.Save("v2"));

        var last = caretaker.PopLast();

        Assert.NotNull(last);
        Assert.Equal("v2", last!.Label);
        Assert.Equal(1, caretaker.Count);
    }

    [Fact]
    public void RewindFlow_RestoresAcrossMultipleSnapshots()
    {
        var draft = new OrderDraft();
        var caretaker = new OrderDraftCaretaker();

        draft.AddLine("A", 1, 1m, 0.1m);
        caretaker.Push(draft.Save("step-1"));
        draft.AddLine("B", 1, 2m, 0.1m);
        caretaker.Push(draft.Save("step-2"));
        draft.AddLine("C", 1, 3m, 0.1m);

        var step1 = caretaker.FindByLabel("step-1")!;
        draft.Restore(step1);

        Assert.Single(draft.Lines);
        Assert.Equal("A", draft.Lines[0].ProductName);
    }

    [Fact]
    public void Save_EmptyLabel_Throws()
    {
        var draft = new OrderDraft();
        Assert.Throws<ArgumentException>(() => draft.Save(""));
    }

    [Fact]
    public void Restore_NullMemento_Throws()
    {
        var draft = new OrderDraft();
        Assert.Throws<ArgumentNullException>(() => draft.Restore(null!));
    }
}
