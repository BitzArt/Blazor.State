using Microsoft.AspNetCore.Components;

namespace BitzArt.Blazor.State.SampleApp.Client.Components;

public partial class Counter : PersistentComponentBase
{
    [Parameter, EditorRequired]
    public string Name { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string? Description { get; set; }

    [Parameter]
    public EventCallback<object> OnStateInitialized { get; set; }
    
    [ComponentState]
    private int _count = 0;

    [ComponentState]
    private string _infoText = string.Empty;

    protected override async Task InitializeStateAsync()
    {
        await Task.Delay(100); // simulate loading data

        _infoText = $"State initialized in render mode: {RendererInfo.Name}";

        if (OnStateInitialized.HasDelegate) await OnStateInitialized.InvokeAsync(this);
    }

    private void IncrementCount(object? state)
    {
        _count++;
        StateHasChanged();
    }
}
