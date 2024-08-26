using Microsoft.AspNetCore.Components;

namespace BitzArt.Blazor.State.SampleApp.Client.Components;

public partial class Counter : PersistentComponentBase, IDisposable
{
    [Parameter, EditorRequired]
    public string Name { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string? Description { get; set; }

    [Parameter]
    public bool EnableTimer { get; set; } = false;

    [ComponentState]
    public int Count { get; private set; } = 0;

    private Timer? _timer;

    private string _stateText = string.Empty;

    protected override async Task InitializeStateAsync()
    {
        await Task.Delay(5000); // simulate loading data
        _stateText = $"State initialized on: {RendererInfo.Name}";
    }

    protected override void OnInitialized()
    {
        if (EnableTimer) _timer = new Timer(IncrementCount, null, 1000, 1000);
    }

    private void IncrementCount(object? state)
    {
        Count++;
        StateHasChanged();
    }

    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
