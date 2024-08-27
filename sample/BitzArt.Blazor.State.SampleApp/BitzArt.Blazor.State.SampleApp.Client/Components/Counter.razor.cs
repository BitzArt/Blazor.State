using Microsoft.AspNetCore.Components;

namespace BitzArt.Blazor.State.SampleApp.Client.Components;

public partial class Counter : PersistentComponent
{
    [Parameter, EditorRequired]
    public string Name { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string? Description { get; set; }

    [ComponentState]
    public int Count { get; private set; } = 0;

    private string _stateText = string.Empty;

    protected override async Task InitializeStateAsync()
    {
        await Task.Delay(100); // simulate loading data
        _stateText = $"State initialized in render mode: {RendererInfo.Name}";
    }

    private void IncrementCount(object? state)
    {
        Count++;
        StateHasChanged();
    }
}
