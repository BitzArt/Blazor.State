using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BitzArt.Blazor.State;

internal class PersistentPageRenderStrategy(PersistentComponent component)
    : PersistentComponentRenderStrategy(component)
{
    protected override bool ShouldWaitForRootStateRestore => false;

    internal override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Wraps this page's contents in a CascadingValue
        // to provide access to this page as a StateRoot
        // for all child components

        PersistentComponent.StateRoot = PersistentComponent;
        PersistentComponent.IsStateRoot = true;

        builder.OpenComponent(0, typeof(CascadingValue<PersistentComponent>));

        builder.AddAttribute(1, "Name", "StateRoot");
        builder.AddAttribute(2, "Value", PersistentComponent);
        builder.AddAttribute(3, "ChildContent",
            (RenderFragment)(innerBuilder => base.BuildRenderTree(innerBuilder)));

        builder.CloseComponent();
    }
}