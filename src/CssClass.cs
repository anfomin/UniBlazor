using System.Text;

namespace UniBlazor;

/// <summary>
/// Represents a builder for creating CSS classes string.
/// </summary>
public struct CssClass
{
	StringBuilder? _string;

	/// <summary>
	/// Initializes a new instance of the <see cref="CssClass"/> with specified value.
	/// </summary>
	/// <param name="value">The initial CSS class value.</param>
	public CssClass(string? value)
	{
		Add(value);
	}

	/// <summary>
	/// Creates an empty <see cref="CssClass"/> instance.
	/// </summary>
	public static CssClass Empty() => new();

	/// <summary>
	/// Creates a <see cref="CssClass"/> instance with the specified value.
	/// </summary>
	/// <param name="value">The initial CSS class value.</param>
	public static CssClass Default(string? value) => new(value);

	/// <summary>
	/// Adds a CSS class to the builder with a space separator.
	/// </summary>
	/// <param name="value">The CSS class to add.</param>
	public CssClass Add(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return this;
		if (_string == null)
			_string = new(value);
		else
		{
			_string.Append(' ');
			_string.Append(value);
		}
		return this;
	}

	/// <summary>
	/// Adds a conditional CSS class to the builder with a space separator.
	/// </summary>
	/// <param name="value">The CSS class to conditionally add.</param>
	/// <param name="when">The condition in which the CSS class is added.</param>
	public CssClass Add(string? value, bool when)
		=> when ? Add(value) : this;

	/// <summary>
	/// Adds a conditional CSS class to the builder with a space separator.
	/// </summary>
	/// <param name="value">The CSS class to conditionally add.</param>
	/// <param name="when">The nullable condition in which the CSS class is added.</param>
	public CssClass Add(string? value, bool? when)
		=> when == true ? Add(value) : this;

	/// <summary>
	/// Adds a conditional CSS class to the builder with a space separator.
	/// </summary>
	/// <param name="value">The function that returns a conditionally added CSS class.</param>
	/// <param name="when">The condition in which the CSS class is added.</param>
	public CssClass Add(Func<string?> value, bool when)
		=> when ? Add(value()) : this;

	/// <summary>
	/// Adds a conditional CSS class to the builder with a space separator.
	/// </summary>
	/// <param name="value">The function that returns a conditionally added CSS class.</param>
	/// <param name="when">The nullable condition in which the CSS class is added.</param>
	public CssClass Add(Func<string?> value, bool? when)
		=> when == true ? Add(value()) : this;

	/// <summary>
	/// Returns the CSS classes as a string.
	/// If no classes were added, returns <c>null</c>.
	/// </summary>
	public override string? ToString()
		=> _string?.ToString();

	/// <summary>
	/// Implicit conversion from <see cref="CssClass"/> to <see cref="string"/>.
	/// </summary>
	public static implicit operator string?(CssClass cssClass)
		=> cssClass.ToString();
}