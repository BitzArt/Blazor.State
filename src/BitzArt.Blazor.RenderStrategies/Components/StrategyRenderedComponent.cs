using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BitzArt.Blazor;

/// <summary>
/// Represents a component that uses a <see cref="ComponentRenderStrategy"/> to render.
/// </summary>
public abstract class StrategyRenderedComponent : IComponent, IHandleAfterRender, IHandleEvent
{
    /// <summary>
    /// A collection of prerequisites that need to be met before the component lifecycle starts.
    /// </summary>
    protected internal ComponentPrerequisiteCollection Prerequisites { get; } = new();

    /// <summary>
    /// Indicates whether the component has completed own initialization (<see cref="Initialize"/> and <see cref="InitializeAsync"/>).
    /// </summary>
    protected internal bool IsInitialized => RenderStrategy!.IsInitialized;

    /// <summary>
    /// Indicates whether the component has completed own initialization, as well as <see cref="OnInitialized"/> and <see cref="OnInitializedAsync"/>.
    /// </summary>
    protected internal bool IsReady => RenderStrategy!.IsReady;

    /// <summary>
    /// Indicates whether the component should wait for complete initialization
    /// before proceeding with further rendering and initializing descendant components. <br/>
    /// Default value is <c>false</c>.
    /// </summary>
    protected internal virtual bool ShouldWaitForCompleteInitialization => RenderStrategy!.ShouldWaitForCompleteInitialization;

    [Inject]
    internal IServiceProvider ServiceProvider
    {
        get => RenderStrategy!.ServiceProvider;
        set => RenderStrategy!.ServiceProvider = value;
    }

    internal virtual bool ShouldUseDefaultStrategy => true;

    private IComponentRenderStrategy? renderStrategy;

    /// <summary>
    /// Assigned rendering strategy of this component.
    /// </summary>
    public IComponentRenderStrategy? RenderStrategy
    {
        get => renderStrategy;
        protected set
        {
            ArgumentNullException.ThrowIfNull(value);

            renderStrategy = value;
            OnStrategyAssigned(value!);
        }
    }

    /// <summary>
    /// This method can be overridden to perform an action
    /// after the rendering strategy has been assigned
    /// to the component.
    /// </summary>
    protected virtual void OnStrategyAssigned(IComponentRenderStrategy strategy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrategyRenderedComponent"/> class.
    /// </summary>
    public StrategyRenderedComponent()
    {
        if (ShouldUseDefaultStrategy)
        {
            RenderStrategy = new ComponentRenderStrategy(this);
        }
    }

    /// <summary>
    /// Renders the component to the supplied <see cref="RenderTreeBuilder"/>.
    /// </summary>
    /// <param name="builder">A <see cref="RenderTreeBuilder"/> that will receive the render output.</param>
    protected virtual void BuildRenderTree(RenderTreeBuilder builder)
    {
    }

    internal void BuildRenderTreeInternal(RenderTreeBuilder builder)
        => BuildRenderTree(builder);

    /// <summary>
    /// Gets the <see cref="IComponentRenderMode"/> assigned to this component.
    /// </summary>
    protected IComponentRenderMode? AssignedRenderMode => RenderStrategy!.AssignedRenderMode;

    /// <summary>
    /// Returns a flag to indicate whether the component should render.
    /// </summary>
    /// <returns></returns>
    protected virtual bool ShouldRender()
        => true;

    internal bool ShouldRenderInternal()
        => ShouldRender();

    /// <summary>
    /// Gets the <see cref="Microsoft.AspNetCore.Components.RendererInfo"/> the component is running on.
    /// </summary>
    protected RendererInfo RendererInfo => RenderStrategy!.Handle.RendererInfo;

    /// <summary>
    /// Gets the <see cref="ResourceAssetCollection"/> for the application.
    /// </summary>
    protected ResourceAssetCollection Assets => RenderStrategy!.Handle.Assets;

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// </summary>
    protected virtual void Initialize()
    {
    }

    internal void InitializeInternal()
        => Initialize();

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// </summary>
    protected virtual Task InitializeAsync()
        => Task.CompletedTask;

    internal Task InitializeAsyncInternal()
        => InitializeAsync();

    /// <summary>
    /// Method invoked after the component has initialized.
    /// By default, this method still running will not prevent the component from rendering. <br/>
    /// To perform a synchronous action that should be completed before proceeding
    /// to rendering the component and it's descendants,
    /// use <see cref="Initialize">Initialize</see>
    /// </summary>
    protected virtual void OnInitialized()
    {
    }

    internal void OnInitializedInternal()
        => OnInitialized();

    /// <summary>
    /// Method invoked after the component has initialized.
    /// By default, this method still running will not prevent the component from rendering. <br/>
    /// To perform an asynchronous action that should be completed before proceeding
    /// to rendering the component and it's descendants,
    /// use <see cref="InitializeAsync">InitializeAsync</see>
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the operation.</returns>
    protected virtual Task OnInitializedAsync()
        => Task.CompletedTask;

    internal Task OnInitializedInternalAsync()
        => OnInitializedAsync();

    /// <summary>
    /// Sets parameters supplied by the component's parent in the render tree.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>A <see cref="Task"/> that completes when the component has finished updating and rendering itself.</returns>
    /// <remarks>
    /// <para>
    /// Parameters are passed when <see cref="SetParametersAsync(ParameterView)"/> is called. It is not required that
    /// the caller supply a parameter value for all of the parameters that are logically understood by the component.
    /// </para>
    /// <para>
    /// The default implementation of <see cref="SetParametersAsync(ParameterView)"/> will set the value of each property
    /// decorated with <see cref="ParameterAttribute" /> or <see cref="CascadingParameterAttribute" /> that has
    /// a corresponding value in the <see cref="ParameterView" />. Parameters that do not have a corresponding value
    /// will be unchanged.
    /// </para>
    /// </remarks>
    public virtual Task SetParametersAsync(ParameterView parameters)
        => RenderStrategy!.SetParametersAsync(parameters);

    /// <summary>
    /// Method invoked when the component has received parameters from its parent in
    /// the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    protected virtual void OnParametersSet()
    {
    }

    internal void OnParametersSetInternal()
        => OnParametersSet();

    /// <summary>
    /// Method invoked when the component has received parameters from its parent in
    /// the render tree, and the incoming values have been assigned to properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    protected virtual Task OnParametersSetAsync()
        => Task.CompletedTask;

    internal Task OnParametersSetInternalAsync()
        => OnParametersSetAsync();

    /// <summary>
    /// Method invoked after each time the component has rendered interactively and the UI has finished
    /// updating (for example, after elements have been added to the browser DOM). Any <see cref="ElementReference" />
    /// fields will be populated by the time this runs.
    ///
    /// This method is not invoked during prerendering or server-side rendering, because those processes
    /// are not attached to any live browser DOM and are already complete before the DOM is updated.
    /// </summary>
    /// <param name="firstRender">
    /// Set to <c>true</c> if this is the first time <see cref="OnAfterRender(bool)"/> has been invoked
    /// on this component instance; otherwise <c>false</c>.
    /// </param>
    /// <remarks>
    /// The <see cref="OnAfterRender(bool)"/> and <see cref="OnAfterRenderAsync(bool)"/> lifecycle methods
    /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
    /// Use the <paramref name="firstRender"/> parameter to ensure that initialization work is only performed
    /// once.
    /// </remarks>
    protected virtual void OnAfterRender(bool firstRender)
    {
    }

    internal void OnAfterRenderInternal(bool firstRender)
        => OnAfterRender(firstRender);

    /// <summary>
    /// Method invoked after each time the component has been rendered interactively and the UI has finished
    /// updating (for example, after elements have been added to the browser DOM). Any <see cref="ElementReference" />
    /// fields will be populated by the time this runs.
    ///
    /// This method is not invoked during prerendering or server-side rendering, because those processes
    /// are not attached to any live browser DOM and are already complete before the DOM is updated.
    ///
    /// Note that the component does not automatically re-render after the completion of any returned <see cref="Task"/>,
    /// because that would cause an infinite render loop.
    /// </summary>
    /// <param name="firstRender">
    /// Set to <c>true</c> if this is the first time <see cref="OnAfterRender(bool)"/> has been invoked
    /// on this component instance; otherwise <c>false</c>.
    /// </param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    /// <remarks>
    /// The <see cref="OnAfterRender(bool)"/> and <see cref="OnAfterRenderAsync(bool)"/> lifecycle methods
    /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
    /// Use the <paramref name="firstRender"/> parameter to ensure that initialization work is only performed
    /// once.
    /// </remarks>
    protected virtual Task OnAfterRenderAsync(bool firstRender)
        => Task.CompletedTask;

    internal Task OnAfterRenderInternalAsync(bool firstRender)
        => OnAfterRenderAsync(firstRender);

    /// <summary>
    /// Notifies the component that its state has changed. When applicable, this will
    /// cause the component to be re-rendered.
    /// </summary>
    protected void StateHasChanged() => RenderStrategy!.StateHasChanged();

    /// <summary>
    /// Executes the supplied work item on the associated renderer's
    /// synchronization context.
    /// </summary>
    /// <param name="workItem">The work item to execute.</param>
    protected Task InvokeAsync(Action workItem)
        => RenderStrategy!.Handle.Dispatcher.InvokeAsync(workItem);

    /// <summary>
    /// Executes the supplied work item on the associated renderer's
    /// synchronization context.
    /// </summary>
    /// <param name="workItem">The work item to execute.</param>
    protected Task InvokeAsync(Func<Task> workItem)
        => RenderStrategy!.Handle.Dispatcher.InvokeAsync(workItem);

    /// <summary>
    /// Treats the supplied <paramref name="exception"/> as being thrown by this component. This will cause the
    /// enclosing ErrorBoundary to transition into a failed state. If there is no enclosing ErrorBoundary,
    /// it will be regarded as an exception from the enclosing renderer.
    ///
    /// This is useful if an exception occurs outside the component lifecycle methods, but you wish to treat it
    /// the same as an exception from a component lifecycle method.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> that will be dispatched to the renderer.</param>
    /// <returns>A <see cref="Task"/> that will be completed when the exception has finished dispatching.</returns>
    protected Task DispatchExceptionAsync(Exception exception)
        => RenderStrategy!.Handle.DispatchExceptionAsync(exception);

    void IComponent.Attach(RenderHandle renderHandle)
        => RenderStrategy!.Attach(renderHandle);

    Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
    {
        var task = callback.InvokeAsync(arg);
        var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion &&
            task.Status != TaskStatus.Canceled;

        // After each event, we synchronously re-render (unless !ShouldRender())
        // This just saves the developer the trouble of putting "StateHasChanged();"
        // at the end of every event callback.
        StateHasChanged();

        return shouldAwaitTask ?
            RenderStrategy!.CallStateHasChangedOnAsyncCompletion(task) :
            Task.CompletedTask;
    }

    Task IHandleAfterRender.OnAfterRenderAsync()
        => RenderStrategy!.OnAfterRenderAsync();
}
