using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BitzArt.Blazor.State;

internal class BlazorStateBuilder(IServiceCollection serviceCollection) : IBlazorStateBuilder
{
    public IServiceCollection ServiceCollection { get; } = serviceCollection;
    public ICollection<Assembly> AddedAssemblies { get; } = [];
}
