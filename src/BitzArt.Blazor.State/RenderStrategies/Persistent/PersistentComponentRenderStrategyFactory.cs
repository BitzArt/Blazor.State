using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace BitzArt.Blazor.State;

internal static class PersistentComponentRenderStrategyFactory
{
    private static bool _initialized;

    internal static HashSet<Type> Components = null!;
    internal static HashSet<Type> Pages = null!;

    internal static void Initialize(IEnumerable<Assembly> assemblies)
    {
        var components = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IStrategyRenderedComponent).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            .ToList();

        var pages = new List<Type>();
        for (var i = 0; i < components.Count; i++)
        {
            var component = components[i];
            var routeAttribute = component.GetCustomAttribute<RouteAttribute>();
            if (routeAttribute is null) continue;

            pages.Add(component);
            components.RemoveAt(i);
            i--;
        }

        Components = [.. components];
        Pages = [.. pages];

        _initialized = true;
    }

    internal static ComponentRenderStrategy CreateStrategy(PersistentComponent component)
    {
        if (!_initialized) throw new InvalidOperationException("PersistentComponentRenderStrategyFactory is not initialized.");

        if (Pages.Contains(component.GetType()))
            return new PersistentPageRenderStrategy(component);

        return new PersistentComponentRenderStrategy(component);
    }
}
