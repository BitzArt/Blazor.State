namespace BitzArt.Blazor;

/// <summary>
/// A prerequisite for a component to be initialized. <br/>
/// This type of prerequisite requires a callback to notify it of the requirement completion. <br/>
/// For a prerequisite type that automatically checks for completion at a specified interval,
/// see <see cref="AutomaticComponentPrerequisite"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="ComponentPrerequisite"/>.
/// </remarks>
/// <param name="requirement"><inheritdoc cref="Requirement" path="/summary"/></param>
public class ComponentPrerequisite(Func<bool> requirement)
{
    /// <summary>
    /// A function that returns a boolean value indicating
    /// whether the prerequisite is met.
    /// </summary>
    public Func<bool> Requirement { get; set; } = requirement;

    /// <summary>
    /// A function that returns a boolean value indicating
    /// whether this prerequisite should be considered.
    /// </summary>
    public Func<bool>? Constraint { get; set; }

    /// <summary>
    /// Prerequisite timeout in milliseconds.
    /// </summary>
    public int Timeout { get; set; } = 5000;

    private bool _completed = false;

    private bool _waiting = false;

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
    /// <param name="allowComponentInitialization"><inheritdoc cref="AllowComponentInitialization" path="/summary"/></param>
    public ComponentPrerequisite(Func<bool> requirement, Func<bool> constraint, ComponentPrerequisiteCallback callback, bool allowComponentInitialization)
        : this(requirement, constraint, callback)
    {
        AllowComponentInitialization = allowComponentInitialization;
    }

    /// <summary>
    /// <inheritdoc cref="ComponentPrerequisite(Func{bool},Func{bool})"/>
    /// </summary>
    /// <param name="requirement"><inheritdoc cref="Requirement" path="/summary"/></param>/>
    /// <param name="constraint"><inheritdoc cref="Constraint" path="/summary"/></param>
    /// <param name="callback"><inheritdoc cref="Callback" path="/summary"/></param>
    public ComponentPrerequisite(Func<bool> requirement, Func<bool> constraint, ComponentPrerequisiteCallback callback)
        : this(requirement, constraint)
    {
        Callback = callback;
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
    /// <param name="callback"><inheritdoc cref="Callback" path="/summary"/></param>
    /// <param name="allowComponentInitialization"><inheritdoc cref="AllowComponentInitialization" path="/summary"/></param>
    public ComponentPrerequisite(Func<bool> requirement, ComponentPrerequisiteCallback callback, bool allowComponentInitialization)
        : this(requirement, allowComponentInitialization)
    {
        Callback = callback;
        callback.Attach(this);
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
        :this(requirement)
    {
        Constraint = constraint;
    }

    internal async Task EnsureAsync()
    {
        try
        {
            if (!ConstraintMet()) return;

            if (Requirement.Invoke())
            {
                _completed = true;
                return;
            }

            if (_cancellationToken is null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                _waiting = true;
            }

            await EnsureRequirementAsync();
        }
        finally
        {
            _cancellationTokenSource?.Dispose();
            _waiting = false;
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
        if (_completed) return;

        if (Requirement.Invoke())
        {
            _completed = true;
            return;
        }

        // Only dispose the timeout task if it was created in this method invocation.
        var dispose = timeoutTask is null;

        try
        {
            timeoutTask ??= Task.Delay(Timeout, _cancellationToken!.Value);

            await WaitAsync(timeoutTask.IgnoreCancellation());

            if (timeoutTask.Status == TaskStatus.RanToCompletion)
            {
                throw new TimeoutException("The prerequisite has not been met within the specified timeout.");
            }

            if (Requirement.Invoke())
            {
                _completed = true;
                return;
            }

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
    public void NotifyCompletion()
    {
        if (!_waiting || _completed) return;

        _cancellationTokenSource?.Cancel();
    }
}
