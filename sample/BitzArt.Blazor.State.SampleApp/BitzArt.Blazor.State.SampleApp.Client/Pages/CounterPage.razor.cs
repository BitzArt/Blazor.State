namespace BitzArt.Blazor.State.SampleApp.Client.Pages;

public partial class CounterPage : PersistentComponentBase
{
    [ComponentState]
    private string Text = "Not Initialized";

    protected override void InitializeState()
    {
        Text = "Initialized";
    }
}
