
namespace BitzArt.Blazor.State.SampleApp.Client.Pages;

public partial class CounterPage : PersistentComponentBase
{
    public CounterPage()
    {
        Prerequisites.AddAuto(100, () => _descendantsInitializedCount > 0, true);
    }

    [ComponentState]
    private string _stateText = "Not Initialized";

    [ComponentState]
    private string _prerequisitesText = "Waiting for prerequisites...";

    [ComponentState]
    private int _descendantsInitializedCount = 0;

    protected override void Initialize()
    {
        _prerequisitesText = "All prerequisites are met";
    }

    protected override void InitializeState()
    {
        _stateText = "Initialized";
    }

    private void OnDescendantInitialized()
    {
        _descendantsInitializedCount++;
    }
}
