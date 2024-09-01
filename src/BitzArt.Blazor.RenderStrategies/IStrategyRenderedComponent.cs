using Microsoft.AspNetCore.Components;

namespace BitzArt.Blazor.State;

internal interface IStrategyRenderedComponent : IComponent, IHandleAfterRender, IHandleEvent
{
}
