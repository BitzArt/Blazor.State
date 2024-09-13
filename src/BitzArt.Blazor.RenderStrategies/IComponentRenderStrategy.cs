

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BitzArt.Blazor.State;

/// <summary>
/// Represents a render strategy for an ASP.NET Core component.
/// </summary>
public interface IComponentRenderStrategy
{
    internal IServiceProvider ServiceProvider { get; set; }

    internal RenderHandle Handle { get; set; }

    internal RendererInfo RendererInfo { get; set; }

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
