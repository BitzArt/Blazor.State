using BitzArt.Blazor.State.SampleApp.Client.Components;

namespace BitzArt.Blazor.State.SampleApp.Client.Pages;

public partial class CounterPage : PersistentComponentBase
{
    private Counter? _counter1;
    private Counter? Counter1
    {
        get => _counter1;
        set
        {
            _counter1 = value;
            Console.WriteLine("Page: Counter1 ref forwarded");
        }
    }
}
