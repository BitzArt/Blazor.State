using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BitzArt.Blazor.State;

internal class PersistentPageRenderStrategy(PersistentComponentBase component)
    : PersistentComponentRenderStrategy(component)
{
    protected override bool ShouldWaitForRootStateRestore => false;

    private bool _shouldPersistState = false;

    internal override void Attach(RenderHandle renderHandle)
    {
        base.Attach(renderHandle);
        _shouldPersistState = Handle.RendererInfo.Name == "Static";
        Console.WriteLine($"\n{Component.GetType().Name}: Rendering page, renderer: {Handle.RendererInfo.Name}");
    }

    internal override void BuildRenderTree(RenderTreeBuilder builder)
    {
        PersistentComponent.StateRoot = PersistentComponent;
        PersistentComponent.IsStateRoot = true;

        builder.OpenComponent<CascadingValue<PersistentComponentBase>>(0);

        builder.AddAttribute(1, "Name", "StateRoot");
        builder.AddAttribute(2, "Value", PersistentComponent);
        builder.AddAttribute(3, "ChildContent",
            (RenderFragment)(innerBuilder1 =>
            {
                if (_shouldPersistState)
                {
                    innerBuilder1.OpenComponent<PageStateContainer>(4);
                    innerBuilder1.AddAttribute(5, "ChildContent",
                        (RenderFragment)(innerBuilder2 => base.BuildRenderTree(innerBuilder2)));

                    innerBuilder1.CloseComponent();
                }
                else
                {
                    base.BuildRenderTree(innerBuilder1);
                }
            }));

        builder.CloseComponent();
    }
}