using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BitzArt.Blazor.State;

internal class PageStateContainer : StrategyRenderedComponentBase
{
    internal const string PageStateElementId = "page-state";

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
    public PersistentComponentStateComposer StateComposer { get; set; } = null!;

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

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // TODO: State container duplicates when re-rendering! This needs to be fixed

        var stateElement = BuildStateElement();
        if (stateElement is not null)
            builder.AddMarkupContent(1, stateElement);

        builder.OpenComponent(2, typeof(CascadingValue<PageStateContainer>));

        builder.AddAttribute(3, "Value", this);
        builder.AddAttribute(4, "ChildContent", ChildContent);

        builder.CloseComponent();
    }

    private string? BuildStateElement()
    {
        var json = StateRoot is not null ? StateComposer.SerializeState(StateRoot!) : null;
        if (json is null) return null;

        var stateEncoded = Convert.ToBase64String(json);

        return $"<script id=\"{PageStateElementId}\" type=\"text/template\">{stateEncoded}</script>";
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
