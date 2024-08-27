namespace BitzArt.Blazor.State;

/// <summary>
/// Base class for persistent components.
/// </summary>
public abstract class PersistentComponent : StrategyRenderedComponent
{
    internal override ComponentRenderStrategy GetRenderStrategy()
        => PersistentComponentRenderStrategyFactory.CreateStrategy(this);

    /// <summary>
    /// Method invoked to initialize the component's state.
    /// </summary>
    protected virtual void InitializeState()
    {
    }

    internal void InitializeStateInternal()
        => InitializeState();

    /// <summary>
    /// Method invoked to asynchronously initialize the component's state.
    /// </summary>
    protected virtual Task InitializeStateAsync()
        => Task.CompletedTask;

    internal Task InitializeStateInternalAsync()
        => InitializeStateAsync();
}