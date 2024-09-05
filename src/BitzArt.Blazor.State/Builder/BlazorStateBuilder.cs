using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BitzArt.Blazor.State;

internal class BlazorStateBuilder : IBlazorStateBuilder
{
    public IServiceCollection ServiceCollection { get; }
    public ICollection<Assembly> AddedAssemblies { get; } = [];
    public PersistentComponentRenderStrategyFactory PersistentComponentRenderStrategyFactory { get; }

    public BlazorStateBuilder(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;

        PersistentComponentRenderStrategyFactory = new PersistentComponentRenderStrategyFactory();
        serviceCollection.AddSingleton(PersistentComponentRenderStrategyFactory);

        serviceCollection.AddSingleton(new StateJsonSerializerOptions());
    }
}
