using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BitzArt.Blazor;

/// <summary>
/// Represents a render strategy for an ASP.NET Core component.
/// </summary>
public interface IComponentRenderStrategy
{
    internal bool IsInitialized { get; }

    internal bool IsReady { get; }

    internal bool ShouldWaitForCompleteInitialization { get; }

    internal IServiceProvider ServiceProvider { get; set; }

    internal RenderHandle Handle { get; set; }

    internal IComponentRenderMode? AssignedRenderMode { get; }

    internal void Attach(RenderHandle renderHandle);

    internal Task SetParametersAsync(ParameterView parameters);

    internal void StateHasChanged();

    internal Task CallStateHasChangedOnAsyncCompletion(Task task);

    internal Task OnAfterRenderAsync();

    /// <summary>
    /// Configures a custom rendering action.
    /// </summary>
    public void SetCustomRenderFragment(Action<RenderTreeBuilder, RenderFragment> customAction);
}
