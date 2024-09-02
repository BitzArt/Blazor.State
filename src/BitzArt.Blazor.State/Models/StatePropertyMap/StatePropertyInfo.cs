using System.Reflection;

namespace BitzArt.Blazor.State;

internal class StatePropertyInfo(Type componentType, PropertyInfo propertyInfo)
{
    public Type ComponentType { get; set; } = componentType;
    public PropertyInfo PropertyInfo { get; set; } = propertyInfo;
}
