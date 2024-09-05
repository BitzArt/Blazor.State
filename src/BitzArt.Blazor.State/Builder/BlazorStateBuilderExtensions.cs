using System.Reflection;

namespace BitzArt.Blazor.State;

/// <summary>
/// Contains extension methods for <see cref="IBlazorStateBuilder"/>.
/// </summary>
public static class BlazorStateBuilderExtensions
{
    /// <summary>
    /// Adds the assembly containing the specified type to the Blazor.State builder.
    /// </summary>
    public static IBlazorStateBuilder AddAssemblyContaining(this IBlazorStateBuilder blazorStateBuilder, Type type)
    {
        return blazorStateBuilder.AddAssembly(type.Assembly);
    }

    /// <summary>
    /// Adds the assembly containing the specified type to the Blazor.State builder.
    /// </summary>
    public static IBlazorStateBuilder AddAssemblyContaining<T>(this IBlazorStateBuilder blazorStateBuilder)
    {
        return blazorStateBuilder.AddAssembly(typeof(T).Assembly);
    }

    /// <summary>
    /// Adds the specified assemblies to the Blazor.State builder.
    /// </summary>
    public static IBlazorStateBuilder AddAssemblies(this IBlazorStateBuilder blazorStateBuilder, params Assembly[] assemblies)
    {
        return blazorStateBuilder.AddAssemblies(assemblies);
    }

    /// <summary>
    /// Adds the specified assemblies to the Blazor.State builder.
    /// </summary>
    /// <param name="blazorStateBuilder"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IBlazorStateBuilder AddAssemblies(this IBlazorStateBuilder blazorStateBuilder, IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            blazorStateBuilder.AddAssembly(assembly);
        }
        return blazorStateBuilder;
    }

    /// <summary>
    /// Adds the specified assembly to the Blazor.State builder.
    /// </summary>
    public static IBlazorStateBuilder AddAssembly(this IBlazorStateBuilder blazorStateBuilder, Assembly assembly)
    {
        blazorStateBuilder.AddedAssemblies.Add(assembly);
        return blazorStateBuilder;
    }
}
