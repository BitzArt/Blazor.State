using System.Text.Json;
using System.Text.Json.Nodes;

namespace BitzArt.Blazor.State;

internal class PersistentComponentStateComposer(
    PersistentComponentStatePropertyMap map,
    StateJsonSerializerOptions stateSerializerOptions)
{
    private readonly JsonSerializerOptions serializerOptions = stateSerializerOptions.Options;

    internal byte[]? SerializeState(PersistentComponentBase component)
    {
        var rootNode = SerializeStateNode(component);
        if (rootNode is null) return null;

        return JsonSerializer.SerializeToUtf8Bytes(rootNode, serializerOptions);
    }

    private JsonObject? SerializeStateNode(PersistentComponentBase component)
    {
        Console.WriteLine($"Serializing state node: {component.GetType().Name}");

        var node = new JsonObject();

        // Serialize properties
        var stateInfo = map.GetComponentStateInfo(component.GetType());

        foreach (var property in stateInfo.StateProperties)
        {
            var value = property.PropertyInfo.GetValue(component);
            var serializedValue = JsonSerializer.SerializeToNode(
                value, property.PropertyInfo.PropertyType, serializerOptions);

            node.Add(property.PropertyInfo.Name, serializedValue);
        }

        // Serialize descendants
        foreach (var descendant in component.StateDescendants)
        {
            var descendantNode = SerializeStateNode(descendant);
            if (descendantNode is null) continue;

            node.Add($"n__{descendant.PositionIdentifier.Id}", descendantNode);
        }

        if (node.Count == 0) return null;

        return node;
    }
}
