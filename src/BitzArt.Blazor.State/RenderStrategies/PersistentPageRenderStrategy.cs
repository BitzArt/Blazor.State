using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
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

    public override bool ShouldWaitForCompleteInitialization => true;

    private bool _shouldPersistState = false;

    internal PersistentPageState? PageState { get; private set; }

    public override void Attach(RenderHandle renderHandle)
    {
        base.Attach(renderHandle);
        _shouldPersistState = Handle.RendererInfo.Name == "Static";
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

    private protected override async Task<bool> TryRestoringStateAsync()
    {
        var js = ServiceProvider.GetRequiredService<IJSRuntime>();
        var module = await js.InvokeAsync<IJSObjectReference>("import", "./_content/BitzArt.Blazor.State/state.js");

        string? stateBase64 = null;

        try
        {
            stateBase64 = await module.InvokeAsync<string?>("getInnerText", [PageStateElementId]);
        }
        finally
        {
            await module.DisposeAsync();
        }


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

            PersistentComponent.OnStateRestoreFailedInternal();

            return false;
        }

        PageState = RestoreBase64State(stateBase64);

        var state = PageState.GetComponentState([]);

        if (state is null)
        {
            // Root state not found.
            return false;
        }

        RestoreComponentState(state);
        StateInitialized = true;

        PersistentComponent.OnStateRestoredInternal();
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