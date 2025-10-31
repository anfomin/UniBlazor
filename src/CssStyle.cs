using System.Text;

namespace UniBlazor;

/// <summary>
/// Represents a builder for creating CSS styles string.
/// </summary>
public class CssStyle
{
	StringBuilder? _string;

	/// <summary>
	/// Initializes a new instance of the <see cref="CssStyle"/> with specified raw style.
	/// </summary>
	/// <param name="raw">CSS raw style.</param>
	public CssStyle(string? raw = null)
	{
		Add(raw);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CssStyle"/> with specified property and value.
	/// </summary>
	/// <param name="prop">CSS style property.</param>
	/// <param name="value">CSS style value.</param>
	public CssStyle(string prop, string? value)
	{
		Add(prop, value);
	}

	/// <summary>
	/// Creates an empty <see cref="CssStyle"/> instance.
	/// </summary>
	public static CssStyle Empty() => new();

	/// <summary>
	/// Creates a <see cref="CssStyle"/> instance with specified raw style.
	/// </summary>
	/// <param name="raw">CSS raw style.</param>
	public static CssStyle Default(string? raw) => new(raw);

	/// <summary>
	/// Creates a <see cref="CssStyle"/> instance with specified property and value.
	/// </summary>
	/// <param name="prop">CSS style property.</param>
	/// <param name="value">CSS style value.</param>
	public static CssStyle Default(string prop, string? value) => new(prop, value);

	/// <summary>
	/// Adds a CSS style to the builder with a semicolon separator.
	/// </summary>
	/// <param name="raw">CSS raw style.</param>
	public CssStyle Add(string? raw)
	{
		if (string.IsNullOrWhiteSpace(raw))
			return this;
		if (_string == null)
			_string = new();
		else
			_string.Append(';');
		_string.Append(raw);
		return this;
	}

	/// <summary>
	/// Adds a conditional CSS style to the builder with a semicolon separator.
	/// </summary>
	/// <param name="raw">CSS raw style to conditionally add.</param>
	/// <param name="when">The condition in which the CSS style is added.</param>
	/// <returns></returns>
	public CssStyle Add(string? raw, bool when)
		=> when ? Add(raw) : this;

	/// <summary>
	/// Adds a conditional CSS style to the builder with a semicolon separator.
	/// </summary>
	/// <param name="raw">CSS raw style to conditionally add.</param>
	/// <param name="when">The nullable condition in which the CSS style is added.</param>
	/// <returns></returns>
	public CssStyle Add(string? raw, bool? when)
		=> when == true ? Add(raw) : this;

	/// <summary>
	/// Adds a conditional CSS style to the builder with a semicolon separator.
	/// </summary>
	/// <param name="raw">The function that returns a conditionally added CSS raw style.</param>
	/// <param name="when">The condition in which the CSS style is added.</param>
	/// <returns></returns>
	public CssStyle Add(Func<string?> raw, bool when)
		=> when ? Add(raw()) : this;

	/// <summary>
	/// Adds a conditional CSS style to the builder with a semicolon separator.
	/// </summary>
	/// <param name="raw">The function that returns a conditionally added CSS raw style.</param>
	/// <param name="when">The nullable condition in which the CSS style is added.</param>
	/// <returns></returns>
	public CssStyle Add(Func<string?> raw, bool? when)
		=> when == true ? Add(raw()) : this;

	/// <summary>
	/// Adds a CSS style property and value to the builder with a semicolon separator.
	/// </summary>
	/// <param name="prop">CSS style property.</param>
	/// <param name="value">CSS style value.</param>
	public CssStyle Add(string prop, string? value)
	{
		if (string.IsNullOrWhiteSpace(prop))
			return this;
		if (_string == null)
			_string = new();
		else
			_string.Append(';');
		_string.Append(prop);
		_string.Append(':');
		_string.Append(value);
		return this;
	}

	/// <summary>
	/// Adds a conditional CSS style property and value to the builder with a semicolon separator.
	/// </summary>
	/// <param name="prop">CSS style property.</param>
	/// <param name="value">CSS style value.</param>
	/// <param name="when">The condition in which the CSS style is added.</param>
	/// <returns></returns>
	public CssStyle Add(string prop, string? value, bool when)
		=> when ? Add(value) : this;

	/// <summary>
	/// Adds a conditional CSS style property and value to the builder with a semicolon separator.
	/// </summary>
	/// <param name="prop">CSS style property.</param>
	/// <param name="value">CSS style value.</param>
	/// <param name="when">The nullable condition in which the CSS style is added.</param>
	/// <returns></returns>
	public CssStyle Add(string prop, string? value, bool? when)
		=> when == true ? Add(value) : this;

	/// <summary>
	/// Adds a conditional CSS style property and value to the builder with a semicolon separator.
	/// </summary>
	/// <param name="prop">CSS style property.</param>
	/// <param name="value">The function that returns style value.</param>
	/// <param name="when">The condition in which the CSS style is added.</param>
	/// <returns></returns>
	public CssStyle Add(string prop, Func<string?> value, bool when)
		=> when ? Add(prop, value()) : this;

	/// <summary>
	/// Adds a conditional CSS style property and value to the builder with a semicolon separator.
	/// </summary>
	/// <param name="prop">CSS style property.</param>
	/// <param name="value">The function that returns style value.</param>
	/// <param name="when">The nullable condition in which the CSS style is added.</param>
	/// <returns></returns>
	public CssStyle Add(string prop, Func<string?> value, bool? when)
		=> when == true ? Add(prop, value()) : this;

	/// <summary>
	/// Returns the CSS styles as a string.
	/// If no styles were added, returns <c>null</c>.
	/// </summary>
	public override string? ToString()
		=> _string?.ToString();

	/// <summary>
	/// Implicit conversion from <see cref="CssStyle"/> to <see cref="string"/>.
	/// </summary>
	public static implicit operator string?(CssStyle cssStyle)
		=> cssStyle.ToString();
}