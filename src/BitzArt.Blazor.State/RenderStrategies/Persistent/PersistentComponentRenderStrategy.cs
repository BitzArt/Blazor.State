namespace BitzArt.Blazor.State;

internal class PersistentComponentRenderStrategy(PersistentComponent component)
    : ComponentRenderStrategy(component)
{
    private bool _stateInitialized;

    protected PersistentComponent PersistentComponent { get; private set; } = component;

    protected override async Task RunInitAndSetParametersAsync()
    {
        // Setup state before initializing the component
        await SetupStateAsync();

        await base.RunInitAndSetParametersAsync();
    }

    protected virtual async Task SetupStateAsync()
    {
        if (_stateInitialized) throw new InvalidOperationException("State has already been initialized for this component.");

        // TODO: Implement state persistence
        // For now, we just initialize the state
        PersistentComponent.InitializeStateInternal();
        await PersistentComponent.InitializeStateInternalAsync();

        _stateInitialized = true;
    }
}
