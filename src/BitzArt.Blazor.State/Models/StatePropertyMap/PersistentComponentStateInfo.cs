using System.Reflection;

namespace BitzArt.Blazor.State;

internal class PersistentComponentStateInfo(Type componentType)
{
    public Type ComponentType { get; set; } = componentType;

    public IEnumerable<StatePropertyInfo> StateProperties { get; set; }
        = componentType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ComponentStateAttribute>() != null)
            .Select(p => new StatePropertyInfo(componentType, p))
            .ToList();

    public IEnumerable<StateFieldInfo> StateFields { get; set; }
        = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.GetCustomAttribute<ComponentStateAttribute>() != null)
            .Select(f => new StateFieldInfo(componentType, f))
            .ToList();
}
