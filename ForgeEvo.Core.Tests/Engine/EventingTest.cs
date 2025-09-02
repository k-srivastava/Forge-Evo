using System;
using ForgeEvo.Core.Engine;
using JetBrains.Annotations;
using Xunit;

namespace ForgeEvo.Core.Tests.Engine;

[TestSubject(typeof(Event))]
public class EventTest
{
    [Fact]
    public void Create_And_Get_Works()
    {
        Event a = EventBus.Create("<Test-A>");
        Event b = EventBus.Create("<Test-B>");

        Assert.Equal(a, EventBus.GetByName("<Test-A>"));
        Assert.Equal(b, EventBus.GetById(b.Id));
    }

    [Fact]
    public void Duplicate_Name_Throws()
    {
        EventBus.Create("<Duplicate>");
        Assert.Throws<InvalidOperationException>(() => EventBus.Create("<Duplicate>"));
    }

    [Fact]
    public void Get_Delete_ByName_And_ById_Works()
    {
        EventBus.Create("<Delete-By-Name>");
        EventBus.DeleteByName("<Delete-By-Name>");
        Assert.False(EventBus.TryGetByName("<Delete-By-Name>", out _));

        Event @event = EventBus.Create("<Delete-By-Id>");
        int id = @event.Id;

        EventBus.DeleteById(id);
        Assert.False(EventBus.TryGetById(id, out _));
    }

    [Fact]
    public void Internal_Events_Cannot_Delete_By_Name()
    {
        EventBus.RegisterInternalEvents(InternalEvent.MouseClicked);

        Assert.True(EventBus.TryGetByName(InternalEvent.MouseClicked.ToName(), out _));
        Assert.Throws<InvalidOperationException>(() => EventBus.DeleteByName(InternalEvent.MouseClicked.ToName()));
    }
}