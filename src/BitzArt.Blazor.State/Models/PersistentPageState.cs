using System.Text.Json;
using System.Text.Json.Nodes;

namespace BitzArt.Blazor.State;

internal class PersistentPageState
{
    private StateNode _rootNode;

    public PersistentPageState(string json, JsonSerializerOptions serializerOptions)
    {
        var state = JsonSerializer.Deserialize<JsonObject>(json, serializerOptions)!;

        _rootNode = new StateNode(parent: null, id: null, state);
        BuildStateTree(_rootNode, state);
    }

    public JsonObject? GetComponentState(IEnumerable<string> path)
    {
        if (!path.Any()) return _rootNode.State!;

        var node = _rootNode;
        foreach (var id in path)
        {
            if (!node.Descendants.TryGetValue(id, out var descendant))
                return null;

            node = descendant;
        }

        return node.State;
    }

    private static void BuildStateTree(StateNode parent, JsonObject state)
    {
        for (var i = state.Count - 1; i >= 0; i--)
        {
            var property = state.ElementAt(i);
            if (!property.Key.StartsWith("n__")) continue;

            var nodeId = property.Key.Substring(3);
            var nodeState = (JsonObject)property.Value!;
            var descendant = new StateNode(parent, nodeId, nodeState);
            BuildStateTree(descendant, nodeState);

            state.Remove(property.Key);
            continue;
        }
    }

    private class StateNode
    {
        public StateNode? Parent { get; set; }

        public string? Id { get; set; }
        public JsonObject? State { get; set; }

        public Dictionary<string, StateNode> Descendants { get; set; }

        public StateNode(StateNode? parent, string? id, JsonObject state)
        {
            Parent = parent;

            if (parent is not null && !string.IsNullOrWhiteSpace(id))
                parent.Descendants.Add(id, this);

            Id = id;
            State = state;
            Descendants = [];
        }
    }
}
