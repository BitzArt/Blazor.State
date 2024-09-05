namespace BitzArt.Blazor.State;

/// <summary>
/// Identifies the component's position within it's parent component.
/// </summary>
internal struct PersistentComponentPositionIdentifier
{
    public string Id { get; set; }

    public PersistentComponentPositionIdentifier(string stateId)
    {
        if (string.IsNullOrWhiteSpace(stateId)) throw new ArgumentNullException(nameof(stateId));
        Id = stateId;
    }

    public PersistentComponentPositionIdentifier(Type componentType)
    {
        Id = componentType.Name;
    }
}
