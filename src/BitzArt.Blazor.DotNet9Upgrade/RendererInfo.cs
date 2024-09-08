using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace BitzArt.Blazor;

/// <summary>
/// This will be removed when dotnet 9 is released.
/// </summary>
public sealed class RendererInfo(string name, bool isInteractive)
{
    /// <summary>
    /// This will be removed when dotnet 9 is released.
    /// </summary>
    public bool IsInteractive { get; init; } = isInteractive;

    /// <summary>
    /// This will be removed when dotnet 9 is released.
    /// </summary>
    public string Name { get; init; } = name;

    /// <summary>
    /// This will be removed when dotnet 9 is released.
    /// </summary>
    public static RendererInfo Static => new("Static", false);

    /// <summary>
    /// This will be removed when dotnet 9 is released.
    /// </summary>
    public static RendererInfo Server => new("Server", true);

    /// <summary>
    /// This will be removed when dotnet 9 is released.
    /// </summary>
    public static RendererInfo WebAssembly => new("WebAssembly", true);
}

internal static class RendererInfoExtensions
{
    public static IServiceCollection AddRendererInfo(this IServiceCollection services)
    {
        services.AddScoped(x => x.GetRendererInfo());

        return services;
    }

    private static RendererInfo GetRendererInfo(this IServiceProvider serviceProvider)
    {
        var isBrowser = OperatingSystem.IsBrowser();

        if (isBrowser) return RendererInfo.WebAssembly;

        var isPrerender = serviceProvider.GetIsPrerender();

        return isPrerender ? RendererInfo.Static : RendererInfo.Server;
    }

    private static bool GetIsPrerender(this IServiceProvider serviceProvider)
    {
        var jsRuntime = serviceProvider.GetRequiredService<IJSRuntime>();

        var JSRuntimeType = jsRuntime.GetType();
        if (JSRuntimeType.Name != "RemoteJSRuntime") return false;

        var IsInitializedProperty = jsRuntime.GetType().GetProperty("IsInitialized");
        var isInitialized = IsInitializedProperty?.GetValue(jsRuntime);

        return isInitialized is not true;
    }
}
