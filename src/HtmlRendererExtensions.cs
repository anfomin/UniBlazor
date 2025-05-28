using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace UniBlazor;

public static class HtmlRendererExtensions
{
	/// <summary>
	/// Renders a Blazor component to HTML asynchronously.
	/// </summary>
	/// <param name="parameters">Component parameters.</param>
	/// <typeparam name="TComponent">Component type to render.</typeparam>
	public static Task<string> RenderComponentToHtmlAsync<TComponent>(this HtmlRenderer htmlRenderer, Dictionary<string, object?>? parameters = null)
		where TComponent : IComponent
		=> htmlRenderer.Dispatcher.InvokeAsync(async () =>
		{
			var parameterView = parameters == null ? ParameterView.Empty : ParameterView.FromDictionary(parameters);
			var output = await htmlRenderer.RenderComponentAsync<TComponent>(parameterView);
			return output.ToHtmlString();
		});
}