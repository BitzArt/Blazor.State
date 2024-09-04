using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BitzArt.Blazor.State;

internal class PageStateContainerRenderStrategy(PageStateContainer component)
    : ComponentRenderStrategy(component)
{
    internal const string PageStateElementId = "page-state";

    internal override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent(0, typeof(CascadingValue<PageStateContainer>));

        builder.AddAttribute(1, "Value", component);
        builder.AddAttribute(2, "ChildContent", component.ChildContent);

        builder.CloseComponent();

        var stateElement = BuildStateElement();
        if (stateElement is not null)
            builder.AddMarkupContent(3, stateElement);
    }

    private string? BuildStateElement()
    {
        var json = component.StateRoot is not null ? component.StateComposer.SerializeState(component.StateRoot!) : null;
        if (json is null) return null;

        var stateEncoded = Convert.ToBase64String(json);

        return $"<script id=\"{PageStateElementId}\" type=\"text/template\">{stateEncoded}</script>";
    }
}
