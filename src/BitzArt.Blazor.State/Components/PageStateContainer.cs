using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BitzArt.Blazor.State;

internal class PageStateContainer : StrategyRenderedComponentBase
{
    internal const string PageStateElementId = "page-state";

    internal override ComponentRenderStrategy GetRenderStrategy()
        => new PageStateContainerRenderStrategy(this);

    /// <summary>
    /// Page root component.
    /// </summary>
    [CascadingParameter(Name = "StateRoot")]
    public PersistentComponentBase? StateRoot { get; set; }

    internal bool HasState { get; set; } = false;

    /// <summary>
    /// Child content.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        StateRoot!.StateContainer = this;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent(1, typeof(CascadingValue<PageStateContainer>));

        builder.AddAttribute(2, "Value", this);
        builder.AddAttribute(3, "ChildContent", ChildContent);

        builder.CloseComponent();
    }
}
