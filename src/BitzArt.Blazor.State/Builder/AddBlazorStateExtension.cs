using Microsoft.Extensions.DependencyInjection;

namespace BitzArt.Blazor.State;

/// <summary>
/// Extension methods for adding Blazor.State services to the DI container.
/// </summary>
public static class AddBlazorStateExtension
{
    /// <summary>
    /// Adds Blazor.State services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained. </returns>
    public static IServiceCollection AddBlazorState(this IServiceCollection services, Action<IBlazorStateBuilder>? configure = null)
    {
        var builder = new BlazorStateBuilder(services);

        if (configure is not null) configure(builder);

        var componentTypes = builder.AddedAssemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsAbstract)
            .Where(type => type.IsAssignableTo(typeof(PersistentComponentBase)))
            .ToList();

        builder.PersistentComponentRenderStrategyFactory.Initialize(componentTypes);

        var componentStatePropertyMap = new PersistentComponentStatePropertyMap(componentTypes);
        services.AddSingleton(componentStatePropertyMap);

        services.AddSingleton<PersistentComponentStateComposer>();

        return services;
    }
}
