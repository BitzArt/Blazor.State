namespace BitzArt.Blazor;

/// <summary>
/// A prerequisite for a component to be initialized. <br/>
/// This type of prerequisite requires a callback to notify it of the requirement completion. <br/>
/// For a prerequisite type that automatically checks for completion at a specified interval,
/// see <see cref="AutomaticComponentPrerequisite"/>.
/// </summary>
public class ComponentPrerequisite
{
    /// <summary>
    /// A function that returns a boolean value indicating
    /// whether the prerequisite is met.
    /// </summary>
    public Func<bool> Requirement { get; set; }

    /// <summary>
    /// A function that returns a boolean value indicating
    /// whether this prerequisite should be considered.
    /// </summary>
    public Func<bool>? Constraint { get; set; }

    /// <summary>
    /// Prerequisite timeout in milliseconds.
    /// </summary>
    public int Timeout { get; set; } = 5000;

    /// <summary>
    /// Determines whether this prerequisite allows initialization of the component
    /// even if the prerequisite is not met. <br/><br/>
    /// If true, the component will be initialized regardless of the prerequisite status,
    /// but the further lifecycle of the component
    /// (such as <see cref="StrategyRenderedComponent.OnInitialized"><c>OnInitialized</c></see>,
    /// <see cref="StrategyRenderedComponent.OnInitializedAsync"><c>OnInitializedAsync</c></see>,
    /// or component rendering) will be determined by the prerequisite status
    /// and will not start until the prerequisite is met.<br/><br/>
    /// Default value is <c>false</c>.
    /// </summary>
    public bool AllowComponentInitialization { get; set; } = false;

    /// <summary>
    /// Prerequisite callback.
    /// </summary>
    internal ComponentPrerequisiteCallback? Callback { get; set; }

    private CancellationTokenSource? _cancellationTokenSource;

    private protected CancellationToken? _cancellationToken;

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool},Func{bool})"/>
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="Requirement" path="/summary"/></param>/>
    /// <param name="constraint"><inheritdoc cref="Constraint" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="Callback" path="/summary"/></param>
    public ComponentPrerequisite(Func<bool> requirement, Func<bool> constraint, ComponentPrerequisiteCallback callback)
        : this(requirement, constraint)
    {
        callback.Attach(this);
    }

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool},Func{bool})"/>
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="Constraint" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="AllowComponentInitialization" path="/summary"/></param>
    public ComponentPrerequisite(Func<bool> requirement, Func<bool> constraint, bool allowComponentInitialization)
        : this(requirement, constraint)
    {
        AllowComponentInitialization = allowComponentInitialization;
    }

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool})"/>
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="Requirement" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="AllowComponentInitialization" path="/summary"/></param>
    public ComponentPrerequisite(Func<bool> requirement, bool allowComponentInitialization)
        : this(requirement)
    {
        AllowComponentInitialization = allowComponentInitialization;
    }

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool})"/>
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="Requirement" path="/summary"/></param>
    /// <param name="constraint"><inheritdoc cref="Constraint" path="/summary"/></param>
    public ComponentPrerequisite(Func<bool> requirement, Func<bool> constraint)
    {
        Requirement = requirement;
        Constraint = constraint;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ComponentPrerequisite"/>.
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="Requirement" path="/summary"/></param>
    public ComponentPrerequisite(Func<bool> requirement)
    {
        Requirement = requirement;
    }

    internal async Task EnsureAsync()
    {
        try
        {
            if (!ConstraintMet()) return;

            if (Requirement.Invoke()) return;

            if (_cancellationToken is null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
            }

            await EnsureRequirementAsync();
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
        }
    }

    private bool ConstraintMet()
    {
        // If there is no constraint, the requirement is always considered.
        if (Constraint is null) return true;

        return Constraint.Invoke();
    }

    private protected virtual async Task EnsureRequirementAsync(Task? timeoutTask = null)
    {
        if (Requirement.Invoke()) return;

        // Only dispose the timeout task if it was created in this method invocation.
        var dispose = timeoutTask is null;

        try
        {
            timeoutTask ??= Task.Delay(Timeout, _cancellationToken!.Value);

            try
            {
                await WaitAsync(timeoutTask);

                if (timeoutTask.Status == TaskStatus.RanToCompletion)
                {
                    throw new TimeoutException("The prerequisite has not been met within the specified timeout.");
                }
            }
            catch
            {
                if (!timeoutTask.IsCanceled)
                {
                    throw;
                }
            }

            if (Requirement.Invoke()) return;

            await EnsureRequirementAsync(timeoutTask);
        }
        finally
        {
            if (dispose && timeoutTask is not null && timeoutTask.IsCompleted)
                timeoutTask?.Dispose();
        }
    }

    private protected virtual async Task WaitAsync(Task timeoutTask)
    {
        await timeoutTask;
    }

    /// <summary>
    /// Notifies the completion of the prerequisite.
    /// </summary>
    public void NotifyCompeltion()
    {
        _cancellationTokenSource?.Cancel();
    }
}
