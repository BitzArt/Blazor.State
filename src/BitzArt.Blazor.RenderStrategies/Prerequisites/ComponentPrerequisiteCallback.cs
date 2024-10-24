namespace BitzArt.Blazor;

/// <summary>
/// A callback for component prerequisites.<br/>
/// It can be attached to one or multiple prerequisites
/// and then <see cref="Invoke">invoked</see>
/// to notify all of it's attached prerequisites of requirement completion.
/// </summary>
public class ComponentPrerequisiteCallback
{
    private readonly List<ComponentPrerequisite> _prerequisites;

    /// <summary>
    /// Creates a new instance of <see cref="ComponentPrerequisiteCallback"/>.
    /// </summary>
    public ComponentPrerequisiteCallback()
    {
        _prerequisites = [];
    }

    /// <summary>
    /// Notifies all attached prerequisites of requirement completion.
    /// </summary>
    public void Invoke()
    {
        if (_prerequisites.Count == 0) return;

        foreach (var prerequisite in _prerequisites)
        {
            prerequisite.NotifyCompletion();
        }
    }

    internal void Attach(ComponentPrerequisite prerequisite)
    {
        prerequisite.Callback = this;
        _prerequisites.Add(prerequisite);
    }
}
