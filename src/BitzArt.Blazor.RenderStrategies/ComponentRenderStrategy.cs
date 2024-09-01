using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BitzArt.Blazor.State;

internal class ComponentRenderStrategy
{
    private (IComponentRenderMode? mode, bool cached) _renderMode;
    private bool _initialized;
    private bool _hasNeverRendered = true;
    private bool _hasPendingQueuedRender;
    private bool _hasCalledOnAfterRender;

    internal RenderHandle Handle { get; set; }
    private readonly RenderFragment _renderFragment;

    public StrategyRenderedComponentBase Component { get; set; }

    internal IComponentRenderMode? AssignedRenderMode
    {
        get
        {
            if (!_renderMode.cached)
            {
                _renderMode = (Handle.RenderMode, true);
            }

            return _renderMode.mode;
        }
    }

    internal ComponentRenderStrategy(StrategyRenderedComponentBase component)
    {
        Component = component;
        _renderFragment = GetRenderFragmentHandler();
    }

    internal virtual RenderFragment GetRenderFragmentHandler() =>
        builder =>
        {
            OnRenderStarted();
            BuildRenderTree(builder);
            OnRendered();
        };

    internal void OnRenderStarted()
    {
        _hasPendingQueuedRender = false;
        _hasNeverRendered = false;

        Console.WriteLine($"{Component.GetType().Name}: Render started");
    }

    internal void OnRendered()
    {
        Console.WriteLine($"{Component.GetType().Name}: Rendered");
    }

    internal virtual void BuildRenderTree(RenderTreeBuilder builder)
    {
        Component.BuildRenderTreeInternal(builder);
    }

    internal void Attach(RenderHandle renderHandle)
    {
        // This implicitly means a ComponentBase can only be associated with a single
        // renderer. That's the only use case we have right now. If there was ever a need,
        // a component could hold a collection of render handles.
        if (Handle.IsInitialized)
        {
            throw new InvalidOperationException($"The render handle is already set. Cannot initialize a {nameof(ComponentRenderStrategy)} more than once.");
        }

        Handle = renderHandle;
    }

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(Component);
        if (!_initialized)
        {
            _initialized = true;

            return RunInitAndSetParametersAsync();
        }
        else
        {
            return CallOnParametersSetAsync();
        }
    }

    protected virtual async Task RunInitAndSetParametersAsync()
    {
        Component.OnInitializedInternal();
        var task = Component.OnInitializedInternalAsync();

        if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled)
        {
            try
            {
                await task;
            }
            catch // avoiding exception filters for AOT runtime support
            {
                // Ignore exceptions from task cancellations.
                // Awaiting a canceled task may produce either an OperationCanceledException (if produced as a consequence of
                // CancellationToken.ThrowIfCancellationRequested()) or a TaskCanceledException (produced as a consequence of awaiting Task.FromCanceled).
                // It's much easier to check the state of the Task (i.e. Task.IsCanceled) rather than catch two distinct exceptions.
                if (!task.IsCanceled)
                {
                    throw;
                }
            }

            // Don't call StateHasChanged here. CallOnParametersSetAsync should handle that for us.
        }

        await CallOnParametersSetAsync();
    }

    private Task CallOnParametersSetAsync()
    {
        Component.OnParametersSetInternal();
        var task = Component.OnParametersSetInternalAsync();
        // If no async work is to be performed, i.e. the task has already ran to completion
        // or was canceled by the time we got to inspect it, avoid going async and re-invoking
        // StateHasChanged at the culmination of the async work.
        var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion &&
            task.Status != TaskStatus.Canceled;

        // We always call StateHasChanged here as we want to trigger a rerender after OnParametersSet and
        // the synchronous part of OnParametersSetAsync has run.
        StateHasChanged();

        return shouldAwaitTask ?
            CallStateHasChangedOnAsyncCompletion(task) :
            Task.CompletedTask;
    }

    internal async Task CallStateHasChangedOnAsyncCompletion(Task task)
    {
        try
        {
            await task;
        }
        catch // avoiding exception filters for AOT runtime support
        {
            // Ignore exceptions from task cancellations, but don't bother issuing a state change.
            if (task.IsCanceled)
            {
                return;
            }

            throw;
        }

        StateHasChanged();
    }

    internal void StateHasChanged()
    {
        if (_hasPendingQueuedRender)
        {
            return;
        }

        if (_hasNeverRendered || Component.ShouldRenderInternal() || Handle.IsRenderingOnMetadataUpdate)
        {
            _hasPendingQueuedRender = true;

            try
            {
                Handle.Render(_renderFragment);
            }
            catch
            {
                _hasPendingQueuedRender = false;
                throw;
            }
        }
    }

    internal Task OnAfterRenderAsync()
    {
        var firstRender = !_hasCalledOnAfterRender;
        _hasCalledOnAfterRender = true;

        Component.OnAfterRenderInternal(firstRender);

        return Component.OnAfterRenderInternalAsync(firstRender);

        // Note that we don't call StateHasChanged to trigger a render after
        // handling this, because that would be an infinite loop. The only
        // reason we have OnAfterRenderAsync is so that the developer doesn't
        // have to use "async void" and do their own exception handling in
        // the case where they want to start an async task.
    }
}
