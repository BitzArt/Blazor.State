using Microsoft.AspNetCore.Components;

namespace BitzArt.Blazor.State;

/// <summary>
/// A container component that can store the state
/// of a persistent page after it's successful prerendering.
/// </summary>
internal class PageStateContainer : StrategyRenderedComponent
{
    /// <summary>
    /// Page root component.
    /// </summary>
    [CascadingParameter(Name = "StateRoot")]
    public PersistentComponentBase? StateRoot { get; set; }

    internal bool HasState { get; set; } = false;

    [Inject]
    internal PersistentComponentStateComposer StateComposer { get; set; } = null!;

    public PageStateContainer() : base()
    {
        RenderStrategy = new PageStateContainerRenderStrategy(this);
    }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        StateRoot!.StateContainer = this;
    }

    internal async Task RefreshAsync()
    {
        var task = InvokeAsync(StateHasChanged);

        if (!task.IsCanceled && task.Status != TaskStatus.RanToCompletion)
            await task.IgnoreCancellation();
    }
}
