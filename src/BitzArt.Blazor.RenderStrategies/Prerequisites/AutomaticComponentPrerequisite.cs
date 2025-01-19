using System.Diagnostics.CodeAnalysis;

namespace BitzArt.Blazor;

/// <summary>
/// A component prerequisite that automatically checks for completion at a specified interval.<br/>
/// The <see cref="ComponentPrerequisite">manual prerequisites</see> are more performant,
/// so use this type of prerequisite only when periodic checks are required and/or application performance is not a concern.
/// </summary>
public class AutomaticComponentPrerequisite : ComponentPrerequisite
{
    /// <summary>
    /// Interval between each check, in milliseconds.
    /// </summary>
    public int Period { get; set; }

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool},Func{bool})"/>
    /// </summary>
    /// <param name="period"><inheritdoc cref="Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>/>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="ComponentPrerequisite.Callback" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    public AutomaticComponentPrerequisite(int period, Func<bool> requirement, Func<bool> constraint, ComponentPrerequisiteCallback callback, bool allowComponentInitialization)
        : this(period, requirement, constraint, callback)
    {
        AllowComponentInitialization = allowComponentInitialization;
    }

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool},Func{bool})"/>
    /// </summary>
    /// <param name="period"><inheritdoc cref="Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>/>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="ComponentPrerequisite.Callback" path="/summary"/></param>
    public AutomaticComponentPrerequisite(int period, Func<bool> requirement, Func<bool> constraint, ComponentPrerequisiteCallback callback)
        : this(period, requirement, constraint)
    {
        Callback = callback;
        callback.Attach(this);
    }

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool},Func{bool})"/>
    /// </summary>
    /// <param name="period"><inheritdoc cref="Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    public AutomaticComponentPrerequisite(int period, Func<bool> requirement, Func<bool> constraint, bool allowComponentInitialization)
        : this(period, requirement, constraint)
    {
        AllowComponentInitialization = allowComponentInitialization;
    }

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool})"/>
    /// </summary>
    /// <param name="period"><inheritdoc cref="Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="ComponentPrerequisite.Callback" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    public AutomaticComponentPrerequisite(int period, Func<bool> requirement, ComponentPrerequisiteCallback callback, bool allowComponentInitialization)
        : this(period, requirement, allowComponentInitialization)
    {
        Callback = callback;
        callback.Attach(this);
    }

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool})"/>
    /// </summary>
    /// <param name="period"><inheritdoc cref="Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="ComponentPrerequisite.AllowComponentInitialization" path="/summary"/></param>
    public AutomaticComponentPrerequisite(int period, Func<bool> requirement, bool allowComponentInitialization)
        : this(period, requirement)
    {
        AllowComponentInitialization = allowComponentInitialization;
    }

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool})"/>
    /// </summary>
    /// <param name="period"><inheritdoc cref="Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="ComponentPrerequisite.Constraint" path="/summary"/></param>
    public AutomaticComponentPrerequisite(int period, Func<bool> requirement, Func<bool> constraint)
        : this(period, requirement)
    {
        Constraint = constraint;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ComponentPrerequisite"/>.
    /// </summary>
    /// <param name="period"><inheritdoc cref="Period" path="/summary"/></param>
    /// <param name="requirement"><inheritdoc cref="ComponentPrerequisite.Requirement" path="/summary"/></param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "In order to have a distinct comment for this constructor")]
    public AutomaticComponentPrerequisite(int period, Func<bool> requirement)
        : base(requirement)
    {
        Period = period;
    }

    private protected override async Task WaitAsync(Task timeoutTask)
    {
        while (!timeoutTask.IsCompleted && !Requirement.Invoke())
        {
            if (timeoutTask.IsCompleted) break;
            var periodTask = Task.Delay(Period, _cancellationToken!.Value);
            await Task.WhenAny(timeoutTask, periodTask);
        }
    }
}
