using System.Reflection;

namespace BitzArt.Blazor.State;

internal class PersistentComponentStateInfo(Type componentType)
{
    public Type ComponentType { get; set; } = componentType;

    public IEnumerable<StatePropertyInfo> StateProperties { get; set; }
        = componentType.GetProperties()
            .Where(p => p.GetCustomAttribute<ComponentStateAttribute>() != null)
            .Select(p => new StatePropertyInfo(componentType, p))
            .ToList();
}
