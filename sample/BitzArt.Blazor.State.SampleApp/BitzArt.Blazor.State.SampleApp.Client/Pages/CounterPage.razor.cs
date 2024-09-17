
namespace BitzArt.Blazor.State.SampleApp.Client.Pages;

public partial class CounterPage : PersistentComponentBase
{
    [ComponentState]
    private string Text = "Not Initialized";

    [ComponentState]
    private int _descendantsInitializedCount = 0;

    [ComponentState]
    private string _prerequisitesText = "Waiting for prerequisites...";

    protected override async Task EnsurePrerequisitesAsync()
    {
        while (_descendantsInitializedCount < 1) await Task.Delay(100);
        _prerequisitesText = "All prerequisites are met";
    }

    protected override void InitializeState()
    {
        Text = "Initialized";
    }

    private void OnDescendantInitialized()
    {
        _descendantsInitializedCount++;
    }
}
