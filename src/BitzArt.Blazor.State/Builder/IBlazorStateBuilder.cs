using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BitzArt.Blazor.State;

/// <summary>
/// Represents a builder for configuring Blazor.State services.
/// </summary>
public interface IBlazorStateBuilder
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where Blazor.State services are registered.
    /// </summary>
    public IServiceCollection ServiceCollection { get; }

    /// <summary>
    /// Gets the assemblies that have been added to the Blazor.State builder.
    /// </summary>
    public ICollection<Assembly> AddedAssemblies { get; }
}
