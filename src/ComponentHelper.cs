using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace UniBlazor;

/// <summary>
/// Provides extension methods for Components.
/// </summary>
public static class ComponentHelper
{
	/// <summary>
	/// Returns label for property <paramref name="expression"/>
	/// from <see cref="DisplayAttribute"/>, <see cref="DisplayNameAttribute"/> or property name.
	/// </summary>
	/// <param name="expression">Property expression.</param>
	public static string Label<T>(Expression<Func<T>> expression)
		=> LabelInternal(expression.Body);

	/// <summary>
	/// Returns label for property <paramref name="expression"/>
	/// from <see cref="DisplayAttribute"/>, <see cref="DisplayNameAttribute"/> or property name.
	/// </summary>
	/// <param name="expression">Property expression.</param>
	public static string LabelFor<T>(Expression<Func<T, object?>> expression)
		=> LabelInternal(expression.Body);

	/// <summary>
	/// Returns label for property <paramref name="expression"/>
	/// from <see cref="DisplayAttribute"/>, <see cref="DisplayNameAttribute"/> or property name.
	/// Adds "*" char is property marked with <see cref="RequiredAttribute"/> or not nullable.
	/// </summary>
	/// <param name="expression">Property expression.</param>
	public static string LabelEdit<T>(Expression<Func<T>> expression)
		=> LabelInternal(expression.Body, edit: true);

	/// <summary>
	/// Returns label for property <paramref name="expression"/>
	/// from <see cref="DisplayAttribute"/>, <see cref="DisplayNameAttribute"/> or property name.
	/// Adds "*" char is property marked with <see cref="RequiredAttribute"/> or not nullable.
	/// </summary>
	/// <param name="expression">Property expression.</param>
	public static string LabelEditFor<T>(Expression<Func<T, object?>> expression)
		=> LabelInternal(expression.Body, edit: true);

	/// <summary>
	/// Returns description for property <paramref name="expression"/>
	/// from <see cref="DisplayAttribute"/> or <see cref="DescriptionAttribute"/>.
	/// </summary>
	/// <param name="expression">Property expression.</param>
	public static string? Description<T>(Expression<Func<T>> expression)
		=> DescriptionInternal(expression.Body);

	/// <summary>
	/// Returns description for property <paramref name="expression"/>
	/// from <see cref="DisplayAttribute"/> or <see cref="DescriptionAttribute"/>.
	/// </summary>
	/// <param name="expression">Property expression.</param>
	public static string? DescriptionFor<T>(Expression<Func<T, object?>> expression)
		=> DescriptionInternal(expression.Body);

	static string LabelInternal(Expression expressionBody, bool edit = false)
	{
		var memberInfo = GetPropertyInformation(expressionBody) ?? throw new ArgumentException("No property reference expression was found", nameof(expressionBody));
		string name = memberInfo.GetCustomAttribute<DisplayAttribute>(true)?.Name
			?? memberInfo.GetCustomAttribute<DisplayNameAttribute>(true)?.DisplayName
			?? memberInfo.Name;
		if (edit && (memberInfo.GetCustomAttribute<RequiredAttribute>(true) != null
				|| memberInfo is PropertyInfo propertyInfo && !propertyInfo.PropertyType.IsNullable()
			))
			name += "*";
		return name;
	}

	static string? DescriptionInternal(Expression expressionBody)
	{
		var memberInfo = GetPropertyInformation(expressionBody) ?? throw new ArgumentException("No property reference expression was found", nameof(expressionBody));
		return memberInfo.GetCustomAttribute<DisplayAttribute>(true)?.Description
			?? memberInfo.GetCustomAttribute<DescriptionAttribute>(true)?.Description
			?? null;
	}

	static MemberInfo? GetPropertyInformation(Expression expression)
		=> expression switch
		{
			MemberExpression memberExpr => memberExpr.Member,
			UnaryExpression { NodeType: ExpressionType.Convert, Operand: MemberExpression memberExpr2 } => memberExpr2.Member,
			_ => null
		};

	/// <summary>
	/// Compares two component parameter dictionaries. Parameters meant equal if they are:
	/// <list type="bullet">
	/// <item>Both <c>null</c>.</item>
	///	<item><see cref="IComparable"/> and compare returns 0.</item>
	/// <item><see cref="EventCallback"/> and <see cref="EventCallback.HasDelegate"/> equals.</item>
	/// <item>Otherwise, <see cref="object.Equals(object?)"/> is used.</item>
	/// </list>
	/// </summary>
	public static bool IsParametersEqual(IReadOnlyDictionary<string, object?> parametersNew, IReadOnlyDictionary<string, object?>? parametersPrev)
		=> parametersNew.Count == parametersPrev?.Count
			&& parametersNew.All(
				p => parametersPrev.TryGetValue(p.Key, out var prevValue) && IsParameterValueEqual(p.Value, prevValue)
			);

	/// <summary>
	/// Compares two component parameters. Parameters meant equal if they are:
	/// <list type="bullet">
	/// <item>Both <c>null</c>.</item>
	///	<item><see cref="IComparable"/> and compare returns 0.</item>
	/// <item><see cref="EventCallback"/> and <see cref="EventCallback.HasDelegate"/> equals.</item>
	/// <item>Otherwise, <see cref="object.Equals(object?)"/> is used.</item>
	/// </list>
	/// </summary>
	public static bool IsParameterValueEqual(object? value1, object? value2)
		=> (value1, value2) switch
		{
			(null, null) => true,
			(null, _) or (_, null) => false,
			(IComparable c1, IComparable c2) => c1.CompareTo(c2) == 0,
			(EventCallback c1, EventCallback c2) => c1.HasDelegate == c2.HasDelegate, // do not compare event callback delegates
			_ => value1.Equals(value2)
		};

	/// <summary>
	/// Adds "@2x" suffix to the file path.
	/// </summary>
	/// <param name="path">File path to add suffix to.</param>
	public static string Get2X(string path)
	{
		var (name, ext) = PathHelper.SplitExtension(path);
		return $"{name}@2x{ext}";
	}
}