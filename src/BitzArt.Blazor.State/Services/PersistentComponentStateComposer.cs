using System.Text.Json;
using System.Text.Json.Nodes;

namespace BitzArt.Blazor.State;

internal class PersistentComponentStateComposer(
    PersistentComponentStatePropertyMap map,
    StateJsonSerializerOptions stateSerializerOptions)
{
    private JsonSerializerOptions serializerOptions = stateSerializerOptions.Options;

    internal byte[] SerializeState(PersistentComponentBase component)
    {
        var rootNode = SerializeStateNode(component);

        return JsonSerializer.SerializeToUtf8Bytes(rootNode, serializerOptions);
    }

    private JsonNode SerializeStateNode(PersistentComponentBase component)
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
            node.Add($"n__{descendant.PositionIdentifier.Id}", SerializeStateNode(descendant));

        return node;
    }
}
