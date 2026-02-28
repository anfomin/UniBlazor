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
	/// <param name="accessor">Field accessor whose value has been changed.</param>
	public static void NotifyFieldChanged<TField>(this EditContext editContext, Expression<Func<TField>> accessor)
		=> editContext.NotifyFieldChanged(FieldIdentifier.Create(accessor));

	/// <summary>
	/// Clears any modification flag that may be tracked for the specified field.
	/// </summary>
	/// <param name="accessor">Field accessor whose modification flag should be cleared.</param>
	public static void MarkAsUnmodified<TField>(this EditContext editContext, Expression<Func<TField>> accessor)
		=> editContext.MarkAsUnmodified(FieldIdentifier.Create(accessor));

	/// <summary>
	/// Determines whether the specified fields in this <see cref="EditContext" /> has been modified.
	/// </summary>
	/// <param name="accessor">Field accessor whose modification flag should be returned.</param>
	/// <returns>True if the field has been modified; otherwise false.</returns>
	public static bool IsModified<TField>(this EditContext editContext, Expression<Func<TField>> accessor)
		=> editContext.IsModified(FieldIdentifier.Create(accessor));

	/// <summary>
	/// Determines whether the specified fields in this <see cref="EditContext" /> has no associated validation messages.
	/// </summary>
	/// <param name="accessor">Identifies the field whose current validation messages should be returned.</param>
	/// <returns>True if the field has no associated validation messages after validation; otherwise false.</returns>
	public static bool IsValid<TField>(this EditContext editContext, Expression<Func<TField>> accessor)
		=> editContext.IsValid(FieldIdentifier.Create(accessor));

	/// <summary>
	/// Gets the current validation messages for the specified field.
	/// This method does not perform validation itself. It only returns messages determined by previous validation actions.
	/// </summary>
	/// <param name="accessor">Identifies the field whose current validation messages should be returned.</param>
	/// <returns>The current validation messages for the specified field.</returns>
	public static IEnumerable<string> GetValidationMessages<TField>(this EditContext editContext, Expression<Func<TField>> accessor)
		=> editContext.GetValidationMessages(FieldIdentifier.Create(accessor));

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