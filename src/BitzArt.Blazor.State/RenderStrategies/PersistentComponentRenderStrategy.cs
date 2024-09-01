using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Threading.Tasks;

namespace BitzArt.Blazor.State;

internal class PersistentComponentRenderStrategy(PersistentComponentBase component)
    : ComponentRenderStrategy(component)
{
    internal bool StateInitialized;

    protected PersistentComponentBase PersistentComponent { get; private set; } = component;
    protected virtual bool ShouldWaitForRootStateRestore => true;

    protected override async Task RunInitAndSetParametersAsync()
    {
        // Setup state before initializing the component
        await SetupStateAsync();

        await base.RunInitAndSetParametersAsync();
    }

    private async Task WaitForPageStateAsync()
    {
        // This should not normally happen
        if (PersistentComponent.IsStateRoot) return;

        if (PersistentComponent.StateRoot!.StateInitialized) return;

        Console.WriteLine($"{PersistentComponent.GetType().Name}: Waiting for root component");

        var cts = new CancellationTokenSource();
        PersistentComponent.StateRoot!.OnStateRestored += cts.Cancel;
        var timeoutTask = Task.Delay(5000, cts.Token);
        if (cts.IsCancellationRequested) return;

        try
        {
            await timeoutTask;
        }
        catch
        {
            if (!timeoutTask.IsCanceled)
            {
                throw;
            }
        }

        if (timeoutTask.Status == TaskStatus.RanToCompletion) throw new TimeoutException("Timed out waiting for root state restore");
    }

    protected virtual async Task SetupStateAsync()
    {
        if (StateInitialized) throw new InvalidOperationException("State has already been initialized for this component.");

        // TODO: Probably needs a better way to determine
        // whether the state should be restored or initialized
        if (Handle.RendererInfo.IsInteractive)
        {
            await RestoreStateAsync();
        }
        else
        {
            await InitializeStateAsync();
        }
    }

    private async Task RestoreStateAsync()
    {
        if (ShouldWaitForRootStateRestore)
        {
            await WaitForPageStateAsync();
        }

        Console.WriteLine($"{PersistentComponent.GetType().Name}: State restore");

        // TODO: State restore logic
        // Just re-initializing the state for now
        await InitializeStateAsync();

        PersistentComponent.NotifyStateRestored();
    }

    private async Task InitializeStateAsync()
    {
        PersistentComponent.InitializeStateInternal();
        await PersistentComponent.InitializeStateInternalAsync();

        StateInitialized = true;
    }

    internal override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // Wraps this component's contents in a CascadingValue
        // to provide access to this component as a StateParent
        // for all child components

        builder.OpenComponent(0, typeof(CascadingValue<PersistentComponentBase>));

        builder.AddAttribute(1, "Name", "StateParent");
        builder.AddAttribute(2, "Value", PersistentComponent);
        builder.AddAttribute(3, "ChildContent",
            (RenderFragment)(innerBuilder => base.BuildRenderTree(innerBuilder)));

        builder.CloseComponent();
    }
}
