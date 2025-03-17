using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.VisualBasic;

namespace UniBlazor;

/// <summary>
/// Provides creating complex types with default constructor and binding their properties from query string.
/// </summary>
public class DefaultComplexBinder : IComplexObjectBinder
{
	public delegate T ParseDelegate<T>(ReadOnlySpan<char> s);
	public delegate bool TryParseDelegate<T>(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out T? result);
	static readonly Dictionary<Type, TryParseDelegate<object>> Parsers = [];

	static bool TryParse(ReadOnlySpan<char> s, out int result) => int.TryParse(s, CultureInfo.InvariantCulture, out result);
	static bool TryParse(ReadOnlySpan<char> s, out long result) => long.TryParse(s, CultureInfo.InvariantCulture, out result);
	static bool TryParse(ReadOnlySpan<char> s, out double result) => double.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
	static bool TryParse(ReadOnlySpan<char> s, out decimal result) => decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
	static bool TryParse(ReadOnlySpan<char> s, out float result) => float.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out result);
	static bool TryParse(ReadOnlySpan<char> s, out DateTime result) => DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
	static bool TryParse(ReadOnlySpan<char> s, out DateOnly result) => DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
	static bool TryParse(ReadOnlySpan<char> s, out TimeOnly result) => TimeOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
	static bool TryParse(ReadOnlySpan<char> s, out TimeSpan result) => TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out result);

	static DefaultComplexBinder()
	{
		RegisterParser(s => s.ToString());
		RegisterParser<bool>(bool.TryParse);
		RegisterParser<int>(TryParse);
		RegisterParser<long>(TryParse);
		RegisterParser<double>(TryParse);
		RegisterParser<decimal>(TryParse);
		RegisterParser<float>(TryParse);
		RegisterParser<DateTime>(TryParse);
		RegisterParser<DateOnly>(TryParse);
		RegisterParser<TimeOnly>(TryParse);
		RegisterParser<TimeSpan>(TryParse);
		RegisterParser<Guid>(Guid.TryParse);
	}

	/// <summary>
	/// Registers a parser with try-parse notation.
	/// </summary>
	public static void RegisterParser<T>(TryParseDelegate<T> parser)
		where T : notnull
	{
		Parsers[typeof(T)] = (ReadOnlySpan<char> s, [MaybeNullWhen(false)] out object? result) =>
		{
			if (parser(s, out var value))
			{
				result = value;
				return true;
			}
			result = default;
			return false;
		};
	}

	/// <summary>
	/// Registers a parser with parse notation wrapped by try-catch for <see cref="FormatException"/> .
	/// </summary>
	public static void RegisterParser<T>(ParseDelegate<T> parser)
		where T : notnull
	{
		Parsers[typeof(T)] = (ReadOnlySpan<char> s, [MaybeNullWhen(false)] out object? result) =>
		{
			try
			{
				result = parser(s);
				return true;
			}
			catch (FormatException)
			{
				result = null;
				return false;
			}
		};
	}

	static bool TryGetParserForIParsable(Type type, [NotNullWhen(true)] out TryParseDelegate<object>? parser)
	{
		var parsable = typeof(IParsable<>).MakeGenericType(type);
		if (!type.IsAssignableTo(parsable))
		{
			parser = null;
			return false;
		}

		string name = nameof(IParsable<>.TryParse);
		string nameEnding = $".{name}";
		var typeRef = type.MakeByRefType();
		var tryParseMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).First(m =>
			(m.Name == name || m.Name.EndsWith(nameEnding))
			&& m.ReturnParameter.ParameterType == typeof(bool)
			&& m.GetParameters() is var pars
			&& pars.Length == 3
			&& pars[0].ParameterType == typeof(string)
			&& pars[1].ParameterType == typeof(IFormatProvider)
			&& pars[2].ParameterType == typeRef
		)!;
		parser = (ReadOnlySpan<char> s, [MaybeNullWhen(false)] out object? result) =>
		{
			object?[] parameters = [s.ToString(), null, null];
			bool res = (bool)tryParseMethod.Invoke(null, parameters)!;
			result = res ? parameters[2] : null;
			return res;
		};
		return true;
	}

	/// <inheritdoc />
	public object Create(Type type, Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>>? queryString)
	{
		var obj = Activator.CreateInstance(type)!;
		if (queryString != null)
			Bind(obj, queryString);
		return obj;
	}

	/// <inheritdoc />
	public void Bind(object obj, Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>> queryString)
	{
		foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty))
		{
			if (queryString.TryGetValue(prop.Name.AsMemory(), out var queryValue) && !queryValue.IsEmpty)
			{
				var propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
				if (Parsers.TryGetValue(propertyType, out var parser))
				{
					if (parser(queryValue.Span, out var parserValue))
						prop.SetValue(obj, parserValue);
				}
				else if (propertyType.IsEnum)
				{
					if (Enum.TryParse(propertyType, queryValue.Span, ignoreCase: true, out var enumValue))
						prop.SetValue(obj, enumValue);
				}
				else if (TryGetParserForIParsable(propertyType, out var parsableParser))
				{
					Parsers[propertyType] = parsableParser;
					if (parsableParser(queryValue.Span, out var parsableValue))
						prop.SetValue(obj, parsableValue);
				}
				else
					throw new NotSupportedException($"Type {propertyType} is not supported for query parameters");
			}
		}
	}
}