using Microsoft.AspNetCore.Components;

namespace BitzArt.Blazor.State;

/// <summary>
/// Base class for persistent components.
/// </summary>
public abstract class PersistentComponentBase : StrategyRenderedComponent
{
    internal override bool ShouldUseDefaultStrategy => false;

    /// <summary>
    /// Root stateful component.
    /// </summary>
    [CascadingParameter(Name = "StateRoot")]
    public PersistentComponentBase? StateRoot { get; internal set; }

    /// <summary>
    /// Parent stateful component.
    /// </summary>
    [CascadingParameter(Name = "StateParent")]
    public PersistentComponentBase? StateParent
    {
        get => stateParent;
        internal set
        {
            ArgumentNullException.ThrowIfNull(value);
            stateParent = value;
            value.StateDescendants.Add(this);
        }
    }
    private PersistentComponentBase? stateParent;

    internal HashSet<PersistentComponentBase> StateDescendants = [];

    private PageStateContainer? stateContainer;

    internal PageStateContainer? StateContainer
    {
        get => stateContainer;
        set
        {
            stateContainer = value;
            PersistentRenderStrategy.StateContainer = value;
        }
    }

    internal bool IsStateRoot = false;

    [Inject]
    internal PersistentComponentRenderStrategyFactory RenderStrategyFactory
    {
        get => renderStrategyFactory;
        set
        {
            renderStrategyFactory = value;
            RenderStrategy = value.CreateStrategy(this);
        }
    }
    private PersistentComponentRenderStrategyFactory renderStrategyFactory = null!;

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
        => StateId is not null ? new(StateId) : new(GetType());

    private PersistentComponentRenderStrategy PersistentRenderStrategy
        => (PersistentComponentRenderStrategy)RenderStrategy!;

    internal bool StateInitialized => PersistentRenderStrategy.StateInitialized;

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

    internal void OnStateRestoreFailedInternal()
    {
        StateRestoreFailed = true;
        OnStateRestoreFailedEvent?.Invoke();
    }

    internal delegate void OnStateRestoreFailedHandler();
    internal event OnStateRestoreFailedHandler? OnStateRestoreFailedEvent;
    internal bool StateRestoreFailed = false;

    internal void OnStateRestoredInternal()
    {
        OnStateRestoredEvent?.Invoke();
        OnStateRestored();
    }

    internal delegate void OnStateRestoredHandler();
    internal event OnStateRestoredHandler? OnStateRestoredEvent;

    /// <summary>
    /// Method invoked after the component's state has been restored.
    /// </summary>
    protected virtual void OnStateRestored()
    {
    }
}