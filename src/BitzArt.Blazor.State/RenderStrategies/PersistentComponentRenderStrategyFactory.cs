using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace BitzArt.Blazor.State;

internal class PersistentComponentRenderStrategyFactory
{
    private bool _initialized;

    internal HashSet<Type> Components = null!;
    internal HashSet<Type> Pages = null!;

    internal void Initialize(IEnumerable<Type> componentTypes)
    {
        var components = new List<Type>(componentTypes);
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

    internal ComponentRenderStrategy CreateStrategy(PersistentComponentBase component)
    {
        if (!_initialized) throw new InvalidOperationException("PersistentComponentRenderStrategyFactory is not initialized.");

        if (Pages.Contains(component.GetType()))
            return new PersistentPageRenderStrategy(component);

        return new PersistentComponentRenderStrategy(component);
    }
}
