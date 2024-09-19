namespace BitzArt.Blazor;

/// <summary>
/// Represents a collection of component prerequisites.
/// </summary>
public sealed class ComponentPrerequisiteCollection
{
    private readonly HashSet<ComponentPrerequisite> _prerequisites = [];

    internal readonly List<ComponentPrerequisite> BeforeInitialize = [];
    internal readonly List<ComponentPrerequisite> AfterInitialize = [];

    /// <summary>
    /// Creates a new instance of <see cref="ComponentPrerequisiteCollection"/>.
    /// </summary>
    public ComponentPrerequisiteCollection()
    {
    }

    internal async Task EnsureBeforeInitializationAsync()
    {
        if (BeforeInitialize.Count == 0) return;
        var tasks = BeforeInitialize.Select(prerequisite => prerequisite.EnsureAsync());
        await Task.WhenAll(tasks);
    }

    internal async Task EnsureAfterInitializationAsync()
    {
        if (AfterInitialize.Count == 0) return;
        var tasks = AfterInitialize.Select(prerequisite => prerequisite.EnsureAsync());
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Adds a <see cref="ComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="ComponentPrerequisite.Callback" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddManual(Func<bool> requirement, Func<bool> constraint, ComponentPrerequisiteCallback callback, bool allowComponentInitialization)
    {
        var prerequisite = new ComponentPrerequisite(requirement, constraint, callback, allowComponentInitialization);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds a <see cref="ComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="ComponentPrerequisite.Callback" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddManual(Func<bool> requirement, Func<bool> constraint, ComponentPrerequisiteCallback callback)
    {
        var prerequisite = new ComponentPrerequisite(requirement, constraint, callback);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds a <see cref="ComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddManual(Func<bool> requirement, Func<bool> constraint, bool allowComponentInitialization)
    {
        var prerequisite = new ComponentPrerequisite(requirement, constraint, allowComponentInitialization);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds a <see cref="ComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="ComponentPrerequisite.Callback" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddManual(Func<bool> requirement, ComponentPrerequisiteCallback callback, bool allowComponentInitialization)
    {
        var prerequisite = new ComponentPrerequisite(requirement, callback, allowComponentInitialization);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds a <see cref="ComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddManual(Func<bool> requirement, bool allowComponentInitialization)
    {
        var prerequisite = new ComponentPrerequisite(requirement, allowComponentInitialization);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds a <see cref="ComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddManual(Func<bool> requirement, Func<bool> constraint)
    {
        var prerequisite = new ComponentPrerequisite(requirement, constraint);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds a <see cref="ComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddManual(Func<bool> requirement)
    {
        var prerequisite = new ComponentPrerequisite(requirement);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds an <see cref="AutomaticComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="period"><inheritdoc cref="AutomaticComponentPrerequisite.Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="ComponentPrerequisite.Callback" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public AutomaticComponentPrerequisite AddAuto(int period, Func<bool> requirement, Func<bool> constraint, ComponentPrerequisiteCallback callback, bool allowComponentInitialization)
    {
        var prerequisite = new AutomaticComponentPrerequisite(period, requirement, constraint, callback, allowComponentInitialization);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds an <see cref="AutomaticComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="period"><inheritdoc cref="AutomaticComponentPrerequisite.Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="ComponentPrerequisite.Callback" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public AutomaticComponentPrerequisite AddAuto(int period, Func<bool> requirement, Func<bool> constraint, ComponentPrerequisiteCallback callback)
    {
        var prerequisite = new AutomaticComponentPrerequisite(period, requirement, constraint, callback);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds an <see cref="AutomaticComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="period"><inheritdoc cref="AutomaticComponentPrerequisite.Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddAuto(int period, Func<bool> requirement, Func<bool> constraint, bool allowComponentInitialization)
    {
        var prerequisite = new AutomaticComponentPrerequisite(period, requirement, constraint, allowComponentInitialization);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds an <see cref="AutomaticComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="period"><inheritdoc cref="AutomaticComponentPrerequisite.Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="ComponentPrerequisite.Callback" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddAuto(int period, Func<bool> requirement, ComponentPrerequisiteCallback callback, bool allowComponentInitialization)
    {
        var prerequisite = new AutomaticComponentPrerequisite(period, requirement, callback, allowComponentInitialization);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds an <see cref="AutomaticComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="period"><inheritdoc cref="AutomaticComponentPrerequisite.Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddAuto(int period, Func<bool> requirement, bool allowComponentInitialization)
    {
        var prerequisite = new AutomaticComponentPrerequisite(period, requirement, allowComponentInitialization);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds an <see cref="AutomaticComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="period"><inheritdoc cref="AutomaticComponentPrerequisite.Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddAuto(int period, Func<bool> requirement, Func<bool> constraint)
    {
        var prerequisite = new AutomaticComponentPrerequisite(period, requirement, constraint);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds an <see cref="AutomaticComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="period"><inheritdoc cref="AutomaticComponentPrerequisite.Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <returns>The added prerequisite.</returns>
    public ComponentPrerequisite AddAuto(int period, Func<bool> requirement)
    {
        var prerequisite = new AutomaticComponentPrerequisite(period, requirement);
        Add(prerequisite);
        return prerequisite;
    }

    /// <summary>
    /// Adds an <see cref="AutomaticComponentPrerequisite"/> to the collection.
    /// </summary>
    /// <param name="prerequisite"></param>
    public void Add(ComponentPrerequisite prerequisite)
    {
        if (_prerequisites.Contains(prerequisite)) throw new InvalidOperationException(
            "The specified prerequisite is already in the collection.");

        _prerequisites.Add(prerequisite);

        Map(prerequisite);
    }

    private void Map(ComponentPrerequisite prerequisite)
    {
        if (prerequisite.AllowComponentInitialization)
        {
            AfterInitialize.Add(prerequisite);
        }
        else
        {
            BeforeInitialize.Add(prerequisite);
        }
    }

    /// <summary>
    /// Removes the specified prerequisite from the collection.
    /// </summary>
    /// <param name="prerequisite"></param>
    public void Remove(ComponentPrerequisite prerequisite)
    {
        var success = _prerequisites.Remove(prerequisite);

        if (!success)throw new InvalidOperationException(
            "The specified prerequisite was not found in the collection.");

        Unmap(prerequisite);
    }

    private void Unmap(ComponentPrerequisite prerequisite)
    {
        if (prerequisite.AllowComponentInitialization)
        {
            AfterInitialize.Remove(prerequisite);
        }
        else
        {
            BeforeInitialize.Remove(prerequisite);
        }
    }
}
