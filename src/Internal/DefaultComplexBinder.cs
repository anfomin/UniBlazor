using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace UniBlazor.Internal;

/// <summary>
/// Provides creating complex types with default constructor and binding their properties from query string.
/// </summary>
public class DefaultComplexBinder(IServiceProvider serviceProvider) : IComplexObjectBinder
{
	public delegate T ParseDelegate<out T>(ReadOnlySpan<char> s);
	public delegate bool TryParseDelegate<T>(ReadOnlySpan<char> s, [MaybeNullWhen(false)] out T result);
	static readonly Dictionary<Type, TryParseDelegate<object>> Parsers = [];
	readonly IServiceProvider _serviceProvider = serviceProvider;

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
		Parsers[typeof(T)] = (s, [MaybeNullWhen(false)] out result) =>
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
		Parsers[typeof(T)] = (s, [MaybeNullWhen(false)] out result) =>
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

	static bool TryCreateParser(Type type, [NotNullWhen(true)] out TryParseDelegate<object>? parser)
	{
		var parsable = typeof(IParsable<>).MakeGenericType(type);
		if (!type.IsAssignableTo(parsable))
		{
			parser = null;
			return false;
		}

		const string name = nameof(IParsable<>.TryParse);
		var typeRef = type.MakeByRefType();
		var staticMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (staticMethods.FirstOrDefault(m =>
			m.Name == name
			&& m.ReturnParameter.ParameterType == typeof(bool)
			&& m.GetParameters() is { Length: 3 } pars
			&& pars[0].ParameterType == typeof(string)
			&& pars[1].ParameterType == typeof(IFormatProvider)
			&& pars[2].ParameterType == typeRef
		) is { } tryParseMethod3)
		{
			parser = (s, [MaybeNullWhen(false)] out result) =>
			{
				object?[] parameters = [s.ToString(), CultureInfo.InvariantCulture, null];
				bool res = (bool)tryParseMethod3.Invoke(null, parameters)!;
				result = res ? parameters[2] : null;
				return res;
			};
		}
		else if (staticMethods.FirstOrDefault(m =>
			m.Name == name
			&& m.ReturnParameter.ParameterType == typeof(bool)
			&& m.GetParameters() is { Length: 2 } pars
			&& pars[0].ParameterType == typeof(string)
			&& pars[1].ParameterType == typeRef
		) is { } tryParseMethod2)
		{
			parser = (s, [MaybeNullWhen(false)] out result) =>
			{
				object?[] parameters = [s.ToString(), null];
				bool res = (bool)tryParseMethod2.Invoke(null, parameters)!;
				result = res ? parameters[1] : null;
				return res;
			};
		}
		else
			throw new InvalidOperationException($"{type}.TryParse method not found.");
		return true;
	}

	public object Create(Type type, Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>>? queryString)
	{
		var obj = ActivatorUtilities.CreateInstance(_serviceProvider, type);
		if (queryString is not null)
			Bind(obj, queryString);
		return obj;
	}

	public void Bind(object obj, Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>> queryString)
	{
		foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty))
		{
			if (!queryString.TryGetValue(prop.Name.AsMemory(), out var queryValue) || queryValue.IsEmpty)
				continue;
			
			var propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
			if (Parsers.TryGetValue(propertyType, out var parser))
			{
				if (parser(queryValue.Span, out var value))
					prop.SetValue(obj, value);
			}
			else if (propertyType.IsEnum)
			{
				if (Enum.TryParse(propertyType, queryValue.Span, ignoreCase: true, out var value))
					prop.SetValue(obj, value);
			}
			else if (TryCreateParser(propertyType, out var newParser))
			{
				Parsers[propertyType] = newParser;
				if (newParser(queryValue.Span, out var value))
					prop.SetValue(obj, value);
			}
			else
				throw new NotSupportedException($"Type {propertyType} is not supported for query parameters");
		}
	}
}