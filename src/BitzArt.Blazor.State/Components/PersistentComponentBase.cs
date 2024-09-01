using Microsoft.AspNetCore.Components;

namespace BitzArt.Blazor.State;

/// <summary>
/// Base class for persistent components.
/// </summary>
public abstract class PersistentComponentBase : StrategyRenderedComponentBase
{
    [CascadingParameter(Name = "StateRoot")]
    internal PersistentComponentBase? StateRoot { get; set; }

    [CascadingParameter(Name = "StateParent")]
    internal PersistentComponentBase? StateParent { get; set; }

    private PageStateContainer? stateContainer;

    [CascadingParameter]
    internal PageStateContainer? StateContainer
    {
        get => stateContainer;
        set
        {
            stateContainer = value;
            PersistentRenderStrategy.StateContainer = value;
            Console.WriteLine($"{GetType().Name}: StateContainer supplied");
        }
    }

    internal bool IsStateRoot = false;

    /// <summary>
    /// Identifies this component within it's closest parent stateful component. <br/><br/>
    /// A unique state identifier is necessary when a single parent stateful component
    /// contains multiple child stateful components of the same type. <br/>
    /// In this case, this parameter can be used to differentiate between such child components. <br/><br/>
    /// Keep in mind that the 'closest parent stateful component',
    /// as well as the 'child stateful components', both refer to <b>stateful</b> components,
    /// not just any components. <br/><br/>
    /// In a stateful component hierarchy, the stateless components are not present and are ignored.
    /// </summary>
    [Parameter]
    public string? StateId { get; set; }

    internal PersistentComponentPositionIdentifier PositionIdentifier
        => StateId is not null ? new (StateId) : new(GetType());

    private PersistentComponentRenderStrategy PersistentRenderStrategy
        => (PersistentComponentRenderStrategy)RenderStrategy;

    internal bool StateInitialized => PersistentRenderStrategy.StateInitialized;

    internal override ComponentRenderStrategy GetRenderStrategy()
        => PersistentComponentRenderStrategyFactory.CreateStrategy(this);

    /// <summary>
    /// Method invoked to initialize the component's state.
    /// </summary>
    protected virtual void InitializeState()
    {
        Console.WriteLine($"{GetType().Name}: InitializeState was called");
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

    internal delegate void OnStateRestoredHandler();
    internal event OnStateRestoredHandler? OnStateRestored;

    internal void NotifyStateRestored()
        => OnStateRestored?.Invoke();
}