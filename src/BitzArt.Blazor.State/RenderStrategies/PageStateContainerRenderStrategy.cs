using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json;

namespace BitzArt.Blazor.State;

internal class PageStateContainerRenderStrategy(PageStateContainer component)
    : ComponentRenderStrategy(component)
{
    internal override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var stateElement = BuildStateElement();
        if (stateElement is not null)
            builder.AddMarkupContent(1, stateElement);

        base.BuildRenderTree(builder);
    }

    private string? BuildStateElement()
    {
        // TODO: Serialize page state
        var json = JsonSerializer.SerializeToUtf8Bytes(JsonSerializer.Deserialize<object>("{}"));

        if (json is null) return null;

        var stateEncoded = Convert.ToBase64String(json);

        return $"<script id=\"{PageStateContainer.PageStateElementId}\" type=\"text/template\">{stateEncoded}</script>";
    }
}
