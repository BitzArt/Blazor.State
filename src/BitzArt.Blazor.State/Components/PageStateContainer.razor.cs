using Microsoft.AspNetCore.Components;

namespace BitzArt.Blazor.State;

internal class PageStateContainer : StrategyRenderedComponentBase
{
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
        try
        {
            if (!task.IsCanceled && task.Status != TaskStatus.RanToCompletion)
                await task;
        }
        catch
        {
            if (!task.IsCanceled) throw;
        }
    }
}
