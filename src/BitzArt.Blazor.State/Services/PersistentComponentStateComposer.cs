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
        var rootNode = SerializeStateNode(component, true);
        if (rootNode is null) return null;

        return JsonSerializer.SerializeToUtf8Bytes(rootNode, serializerOptions);
    }

    private JsonObject? SerializeStateNode(PersistentComponentBase component, bool root)
    {
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

        // Serialize fields
        foreach (var field in stateInfo.StateFields)
        {
            var value = field.FieldInfo.GetValue(component);
            var serializedValue = JsonSerializer.SerializeToNode(
                value, field.FieldInfo.FieldType, serializerOptions);

            node.Add(field.FieldInfo.Name, serializedValue);
        }

        // Proceed to descendants
        foreach (var descendant in component.StateDescendants)
        {
            var descendantNode = SerializeStateNode(descendant, false);
            if (descendantNode is null) continue;

            node.Add($"n__{descendant.PositionIdentifier.Id}", descendantNode);
        }

        if (!root && node.Count == 0) return null;

        return node;
    }
}
