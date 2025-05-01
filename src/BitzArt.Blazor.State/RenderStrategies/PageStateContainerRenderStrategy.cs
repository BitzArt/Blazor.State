using Microsoft.AspNetCore.Components.Rendering;
using static BitzArt.Blazor.State.Constants;

namespace BitzArt.Blazor.State;

internal class PageStateContainerRenderStrategy(PageStateContainer component)
    : ComponentRenderStrategy(component)
{
    internal override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var stateElement = BuildStateElement();
        if (stateElement is null) return;

        builder.AddMarkupContent(0, stateElement);
    }

    private string? BuildStateElement()
    {
        var json = component.StateRoot is not null ? component.StateComposer.SerializeState(component.StateRoot!) : null;
        if (json is null) return null;

        var stateEncoded = Convert.ToBase64String(json);

        return $"<script id=\"{PageStateElementId}\" type=\"text/template\">{stateEncoded}</script>";
    }
}
