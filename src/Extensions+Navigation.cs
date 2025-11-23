using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;

namespace UniBlazor;

public static partial class Extensions
{
	/// <summary>
	/// Returns if <paramref name="uri"/> path ends with <paramref name="suffix"/>.
	/// </summary>
	public static bool IsPathEndsWith(this Uri uri, string suffix)
		=> uri.AbsolutePath.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);

	/// <summary>
	/// Returns if location URI path ends with <paramref name="suffix"/>.
	/// </summary>
	public static bool IsPathEndsWith(this LocationChangedEventArgs args, string suffix)
		=> new Uri(args.Location).IsPathEndsWith(suffix);

	extension(NavigationManager navigation)
	{
		/// <summary>
		/// Returns if current URI path ends with <paramref name="suffix"/>.
		/// </summary>
		public bool IsPathEndsWith(string suffix)
			=> new Uri(navigation.Uri).IsPathEndsWith(suffix);

		/// <summary>
		/// Returns local URI starting with '/'.
		/// </summary>
		public string GetLocalUri()
			=> $"/{navigation.Uri[navigation.BaseUri.Length..].TrimStart('/')}";

		/// <summary>
		/// Return URI with specified <paramref name="path"/> and current query parameters.
		/// </summary>
		/// <param name="path">Destination path.</param>
		public string GetUriPreservingQuery([StringSyntax(StringSyntaxAttribute.Uri)] string path)
		{
			var uri = new Uri(navigation.Uri);
			return navigation.BaseUri + path.TrimStart('/') + uri.Query;
		}

		/// <summary>
		/// Return URI with specified <paramref name="path"/> and new query <paramref name="parameters"/> merged with current query parameters.
		/// </summary>
		/// <param name="path">Destination path.</param>
		/// <param name="parameters">New query parameters.</param>
		public string GetUriPreservingQuery([StringSyntax(StringSyntaxAttribute.Uri)] string path, IReadOnlyDictionary<string, object?> parameters)
		{
			NavParams query = [];
			var queryEnumerable = new QueryStringEnumerable(QueryStringHelper.GetQueryString(navigation.Uri));
			foreach (var pair in queryEnumerable)
			{
				var decodedName = pair.DecodeName();
				var decodedValue = pair.DecodeValue();
				query[decodedName.ToString()] = decodedValue.ToString();
			}
			foreach (var kv in parameters)
				query[kv.Key] = ConvertParameterValue(kv.Value);

			return navigation.GetUriWithQueryParameters(path, query);
		}

		/// <summary>
		/// Navigates to the specified URI and query <paramref name="parameters"/>.
		/// </summary>
		/// <param name="uri">The destination URI. This can be absolute, or relative to the base URI (as returned by <see cref="NavigationManager.BaseUri"/>).</param>
		/// <param name="parameters">Query parameters.</param>
		/// <param name="forceLoad">If <see langword="true"/>, bypasses client-side routing and forces the browser to load the new page from the server, whether or not the URI would normally be handled by the client-side router.</param>
		public void NavigateTo([StringSyntax(StringSyntaxAttribute.Uri)] string uri, IReadOnlyDictionary<string, object?> parameters, bool forceLoad = false)
		{
			var parameters2 = parameters.ToDictionary(kv => kv.Key, kv => (object?)ConvertParameterValue(kv.Value));
			string url = navigation.GetUriWithQueryParameters(uri, parameters2);
			navigation.NavigateTo(url, forceLoad);
		}

		/// <summary>
		/// Navigates to the specified <paramref name="path"/> and current query parameters.
		/// </summary>
		/// <param name="path">Destination path.</param>
		/// <param name="parameters">New query parameters to merge with current ones.</param>
		public void NavigatePreservingQueryTo([StringSyntax(StringSyntaxAttribute.Uri)] string path, IReadOnlyDictionary<string, object?>? parameters = null)
			=> navigation.NavigateTo(parameters is null
				? navigation.GetUriPreservingQuery(path)
				: navigation.GetUriPreservingQuery(path, parameters)
			);

		/// <summary>
		/// Navigates to new query <paramref name="parameters"/>.
		/// </summary>
		/// <param name="parameters">New query parameters.</param>
		public void NavigateToParams(IReadOnlyDictionary<string, object?> parameters)
			=> navigation.NavigateTo(navigation.Uri, parameters);

		/// <summary>
		/// Navigates to new string query parameter.
		/// </summary>
		/// <param name="name">New query parameter name.</param>
		/// <param name="value">Query parameter value.</param>
		public void NavigateToParam(string name, string? value)
		{
			string url = navigation.GetUriWithQueryParameter(name, value?.NullIfEmpty());
			navigation.NavigateTo(url);
		}

		/// <summary>
		/// Navigates to new integer query parameter.
		/// </summary>
		/// <param name="name">New query parameter name.</param>
		/// <param name="value">Query parameter value.</param>
		public void NavigateToParam(string name, int? value)
		{
			string url = navigation.GetUriWithQueryParameter(name, value);
			navigation.NavigateTo(url);
		}

		/// <summary>
		/// Navigates to new boolean query parameter.
		/// </summary>
		/// <param name="name">New query parameter name.</param>
		/// <param name="value">Query parameter value.</param>
		public void NavigateToParam(string name, bool value)
			=> NavigateToParam(navigation, name, value ? "true" : null);

		/// <summary>
		/// Navigates to new date query parameter.
		/// </summary>
		/// <param name="name">New query parameter name.</param>
		/// <param name="value">Query parameter value.</param>
		public void NavigateToParam(string name, DateOnly? value)
			=> NavigateToParam(navigation, name, value?.ToString("o"));

		/// <summary>
		/// Navigates to new query parameter.
		/// </summary>
		/// <param name="name">New query parameter name.</param>
		/// <param name="value">Query parameter value.</param>
		public void NavigateToParam(string name, object? value)
			=> NavigateToParam(navigation, name, ConvertParameterValue(value));
	}

	static string? ConvertParameterValue(object? value) => value switch
	{
		null => null,
		string s => s.NullIfEmpty(),
		bool b => b.ToString(CultureInfo.InvariantCulture),
		int i => i.ToString(CultureInfo.InvariantCulture),
		short s => s.ToString(CultureInfo.InvariantCulture),
		long l => l.ToString(CultureInfo.InvariantCulture),
		double d => d.ToString(CultureInfo.InvariantCulture),
		float f => f.ToString(CultureInfo.InvariantCulture),
		decimal dec => dec.ToString(CultureInfo.InvariantCulture),
		DateTime dt => dt.ToString(CultureInfo.InvariantCulture),
		DateOnly d => d.ToString(CultureInfo.InvariantCulture),
		TimeOnly t => t.ToString(CultureInfo.InvariantCulture),
		Guid g => g.ToString(null, CultureInfo.InvariantCulture),
		_ when value.GetType().IsEnum => ((int)value).ToString(CultureInfo.InvariantCulture),
		_ => value.ToString()
	};
}