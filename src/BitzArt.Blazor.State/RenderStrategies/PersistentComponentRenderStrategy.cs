using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BitzArt.Blazor.State;

internal class PersistentComponentRenderStrategy(PersistentComponentBase component)
    : ComponentRenderStrategy(component)
{
    internal bool StateInitialized;
    internal PageStateContainer? StateContainer { get; set; }

    protected PersistentComponentBase PersistentComponent { get; private set; } = component;
    protected virtual bool ShouldWaitForRootStateRestore => true;

    protected override async Task InitAndSetParametersAsync()
    {
        // TODO: Dotnet 9
        //var isInteractive = Handle.RendererInfo.IsInteractive;
        var isInteractive = RendererInfo.IsInteractive;

        var shouldInitializeState = true;

        if (isInteractive)
        {
            var restored = await TryRestoringStateAsync();
            shouldInitializeState = !restored;
        }

        await base.InitAndSetParametersAsync();

        if (shouldInitializeState) await InitializeStateAsync();

        if (PersistentComponent.StateContainer is not null)
            await PersistentComponent.StateContainer!.RefreshAsync();
    }

    private async Task WaitForPageStateAsync()
    {
        // This should not normally happen
        if (PersistentComponent.IsStateRoot) throw new UnreachableException();

        if (PersistentComponent.StateRoot!.StateInitialized) return;

        using var cts = new CancellationTokenSource();
        PersistentComponent.StateRoot!.OnStateRestoredEvent += cts.Cancel;
        using var timeoutTask = Task.Delay(5000, cts.Token);
        if (cts.IsCancellationRequested) return;
        if (PersistentComponent.StateRoot!.StateInitialized) return;

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

        if (timeoutTask.Status == TaskStatus.RanToCompletion)
            throw new TimeoutException("Timed out: Page state took too long to restore.");
    }

    protected virtual async Task<bool> TryRestoringStateAsync()
    {
        if (ShouldWaitForRootStateRestore)
        {
            await WaitForPageStateAsync();
        }

        var rootStrategy = PersistentComponent.StateRoot!.RenderStrategy;
        if (rootStrategy is not PersistentPageRenderStrategy pageStrategy)
            throw new InvalidOperationException("The root stateful component is not a page. Make sure your page inherits from PersistentComponentBase.");

        var pageState = pageStrategy.PageState;

        if (pageState is null)
        {
            // Page state not found.
            return false;
        }

        var path = GetComponentLocation(PersistentComponent);
        var state = pageState.GetComponentState(path);

        if (state is null)
        {
            // Component state not found.
            return false;
        }
        
        RestoreComponentState(state);
        StateInitialized = true;

        PersistentComponent.OnStateRestoredInternal();
        return true;
    }

    protected void RestoreComponentState(JsonObject state)
    {
        var stateInfo = ServiceProvider
            .GetRequiredService<PersistentComponentStatePropertyMap>()
            .GetComponentStateInfo(PersistentComponent.GetType());

        var serializerOptions = ServiceProvider.GetRequiredService<StateJsonSerializerOptions>().Options;

        foreach (var property in stateInfo.StateProperties)
        {
            var propertyInfo = property.PropertyInfo;
            var value = state[propertyInfo.Name];

            if (value is null) continue;

            var deserializedValue = value.Deserialize(propertyInfo.PropertyType, serializerOptions);
            propertyInfo.SetValue(PersistentComponent, deserializedValue);
        }

        foreach (var field in stateInfo.StateFields)
        {
            var fieldInfo = field.FieldInfo;
            var value = state[fieldInfo.Name];

            if (value is null) continue;

            var deserializedValue = value.Deserialize(fieldInfo.FieldType, serializerOptions);
            fieldInfo.SetValue(PersistentComponent, deserializedValue);
        }
    }

    private IEnumerable<string> GetComponentLocation(PersistentComponentBase component)
    {
        List<string> location = [];

        var current = component;
        while (!current!.IsStateRoot)
        {
            location.Add(current.PositionIdentifier.Id);
            current = current.StateParent;
        }
        location.Reverse();

        return location;
    }

    protected async Task InitializeStateAsync()
    {
        PersistentComponent.InitializeStateInternal();
        await PersistentComponent.InitializeStateInternalAsync();

        StateInitialized = true;
    }

    internal override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent(0, typeof(CascadingValue<PersistentComponentBase>));

        builder.AddAttribute(1, "Name", "StateParent");
        builder.AddAttribute(2, "Value", PersistentComponent);
        builder.AddAttribute(3, "ChildContent",
            (RenderFragment)(innerBuilder => base.BuildRenderTree(innerBuilder)));

        builder.CloseComponent();
    }
}
