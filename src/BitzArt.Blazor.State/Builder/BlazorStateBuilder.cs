using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BitzArt.Blazor.State;

internal class BlazorStateBuilder : IBlazorStateBuilder
{
    public BlazorStateBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
        AddedAssemblies = [];
    }

    public IServiceCollection ServiceCollection { get; }
    public ICollection<Assembly> AddedAssemblies { get; }
}
