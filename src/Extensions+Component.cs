using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace UniBlazor;

public static partial class Extensions
{
	/// <summary>
	/// Signals that the value for the specified field has changed.
	/// </summary>
	/// <param name="editContext">Current edit context.</param>
	/// <param name="accessor">Field accessor whose value has been changed.</param>
	public static void NotifyFieldChanged<TField>(this EditContext editContext, Expression<Func<TField>> accessor)
		=> editContext.NotifyFieldChanged(FieldIdentifier.Create(accessor));

	extension(HtmlRenderer htmlRenderer)
	{
		/// <summary>
		/// Renders a Blazor component to HTML asynchronously.
		/// </summary>
		/// <param name="parameters">Component parameters.</param>
		/// <typeparam name="TComponent">Component type to render.</typeparam>
		public Task<string> RenderComponentToHtmlAsync<TComponent>(Dictionary<string, object?>? parameters = null)
			where TComponent : IComponent
			=> htmlRenderer.Dispatcher.InvokeAsync(async () =>
			{
				var parameterView = parameters is null ? ParameterView.Empty : ParameterView.FromDictionary(parameters);
				var output = await htmlRenderer.RenderComponentAsync<TComponent>(parameterView);
				return output.ToHtmlString();
			});

		/// <summary>
		/// Creates a component builder for the specified component type.
		/// </summary>
		public ComponentBuilder<TComponent> Build<TComponent>()
			where TComponent : IComponent
			=> new(htmlRenderer);
	}
}