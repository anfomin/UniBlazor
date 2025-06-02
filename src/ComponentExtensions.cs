using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace UniBlazor;

/// <summary>
/// Provides extensions for Blazor components.
/// </summary>
public static class ComponentExtensions
{
	/// <summary>
	/// Signals that the value for the specified field has changed.
	/// </summary>
	/// <param name="editContext">Current edit context.</param>
	/// <param name="accessor">Field accessor whose value has been changed.</param>
	public static void NotifyFieldChanged<TField>(this EditContext editContext, Expression<Func<TField>> accessor)
		=> editContext.NotifyFieldChanged(FieldIdentifier.Create(accessor));

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