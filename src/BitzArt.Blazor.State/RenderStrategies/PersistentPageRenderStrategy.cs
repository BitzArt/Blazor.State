﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Text;
using static BitzArt.Blazor.State.Constants;

namespace BitzArt.Blazor.State;

internal class PersistentPageRenderStrategy(PersistentComponentBase component)
    : PersistentComponentRenderStrategy(component)
{
    protected override bool ShouldWaitForRootStateRestore => false;

    private bool _shouldPersistState = false;

    internal PersistentPageState? PageState { get; private set; }

    public override void Attach(RenderHandle renderHandle)
    {
        base.Attach(renderHandle);
        // TODO: Dotnet 9
        //_shouldPersistState = Handle.RendererInfo.Name == "Static";
        //Console.WriteLine($"\n{Component.GetType().Name}: Rendering page, renderer: {Handle.RendererInfo.Name}");
        _shouldPersistState = RendererInfo.Name == "Static";
        Console.WriteLine($"\n{Component.GetType().Name}: Rendering page, renderer: {RendererInfo.Name}");
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

    protected override async Task<bool> TryRestoringStateAsync()
    {
        var js = ServiceProvider.GetRequiredService<IJSRuntime>();
        var stateBase64 = await js.InvokeAsync<string?>("getInnerText", [PageStateElementId]);

        if (stateBase64 is null)
        {
            // TODO: Do not log warning if prerendering is disabled.

            // (Not whether we are currently prerendering or not,
            // but whether prerendering is enabled for this page in general,
            // and thus whether we should expect the state to be present
            // from the page being prerendered previously or not.)

            // I haven't yet found a way to determine this.

            // The current implementation will log a warning while rendering every page
            // that uses an interactive render mode with prerendering disabled.

            ServiceProvider.GetRequiredService<ILogger<PersistentPageRenderStrategy>>()
                .LogWarning("State container was not found on page. Initializing state as a fallback. Ignore this warning if prerendering is disabled for this page.");

            return false;
        }

        PageState = RestoreBase64State(stateBase64);
        StateInitialized = true;

        PersistentComponent.NotifyStateRestored();
        return true;
    }

    private PersistentPageState RestoreBase64State(string base64State)
    {
        if (string.IsNullOrWhiteSpace(base64State))
            throw new ArgumentException("State string is null or empty.", nameof(base64State));

        var buffer = Convert.FromBase64String(base64State);
        var json = Encoding.UTF8.GetString(buffer);

        var stateSerializerOptions = ServiceProvider.GetRequiredService<StateJsonSerializerOptions>().Options;

        return new(json, stateSerializerOptions);
    }
}