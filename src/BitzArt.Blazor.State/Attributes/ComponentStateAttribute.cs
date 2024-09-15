namespace BitzArt.Blazor.State;

/// <summary>
/// Marks a property to be persisted between rendering environments.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ComponentStateAttribute : Attribute
{
}
