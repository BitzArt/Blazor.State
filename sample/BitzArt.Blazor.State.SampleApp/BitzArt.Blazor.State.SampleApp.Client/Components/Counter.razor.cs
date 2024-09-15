using Microsoft.AspNetCore.Components;

namespace BitzArt.Blazor.State.SampleApp.Client.Components;

public partial class Counter : PersistentComponentBase
{
    [Parameter, EditorRequired]
    public string Name { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string? Description { get; set; }

    [ComponentState]
    private int _count = 0;

    [ComponentState]
    private string InfoText = string.Empty;

    protected override async Task InitializeStateAsync()
    {
        await Task.Delay(100); // simulate loading data

        InfoText = $"State initialized in render mode: {RendererInfo.Name}";
    }

    private void IncrementCount(object? state)
    {
        _count++;
        StateHasChanged();
    }
}
