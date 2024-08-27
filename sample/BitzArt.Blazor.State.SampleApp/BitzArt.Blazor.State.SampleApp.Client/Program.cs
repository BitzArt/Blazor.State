using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BitzArt.Blazor.State.SampleApp.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddBlazorState(builder =>
        {
            builder.AddAssemblyContaining<_Imports>();
        });

        await builder.Build().RunAsync();
    }
}
