﻿namespace BitzArt.Blazor.State;

internal class PersistentComponentStatePropertyMap
{
    private readonly Dictionary<Type, PersistentComponentStateInfo> Components;

    public PersistentComponentStatePropertyMap(IEnumerable<Type> componentTypes)
    {
        Components = componentTypes
            .Select(x => new PersistentComponentStateInfo(x))
            .ToDictionary(x => x.ComponentType);
    }

    public PersistentComponentStateInfo GetComponentStateInfo(Type componentType)
    {
        if (!Components.TryGetValue(componentType, out var componentStateInfo))
            throw new InvalidOperationException($"Component {componentType.FullName} is not registered.");
        
        return componentStateInfo;
    }
}
