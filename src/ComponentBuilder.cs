using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace UniBlazor;

/// <summary>
/// Builder for creating and rendering Blazor components with specified parameters.
/// </summary>
public class ComponentBuilder<TComponent>(HtmlRenderer htmlRenderer)
	where TComponent : IComponent
{
	readonly HtmlRenderer _htmlRenderer = htmlRenderer;
	readonly Dictionary<string, object?> _parameters = new();

	/// <summary>
	/// Sets a parameter for the component.
	/// </summary>
	/// <param name="propertyExpression">Expression to select the component property.</param>
	/// <param name="value">Property value.</param>
	public ComponentBuilder<TComponent> SetParameter<TParam>(Expression<Func<TComponent, TParam>> propertyExpression, TParam value)
	{
		if (propertyExpression.Body is not MemberExpression memberExpr)
			throw new ArgumentException("Expression body must be MemberExpression", nameof(propertyExpression));
		string propertyName = memberExpr.Member.Name;
		_parameters[propertyName] = value;
		return this;
	}

	/// <summary>
	/// Renders the component to HTML asynchronously.
	/// </summary>
	public Task<string> RenderToHtmlAsync()
		=> _htmlRenderer.RenderComponentToHtmlAsync<TComponent>(_parameters);
}