using System.Reflection;

namespace BitzArt.Blazor.State;

internal class StateFieldInfo(Type componentType, FieldInfo fieldInfo)
{
    public Type ComponentType { get; set; } = componentType;
    public FieldInfo FieldInfo { get; set; } = fieldInfo;
}