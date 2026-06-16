using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace UniBlazor;

public static partial class Extensions
{
	extension(NavigationManager navigation)
	{
		/// <summary>
		/// Returns current relative URI to <see cref="NavigationManager.BaseUri"/> without leading slash.
		/// </summary>
		public ReadOnlySpan<char> RelativeUri
			=> navigation.Uri.AsSpan()[navigation.BaseUri.Length..];

		/// <summary>
		/// Returns current URI path without leading slash.
		/// </summary>
		public ReadOnlySpan<char> Path
			=> navigation.Uri.AsSpan().IndexOf('?') is int queryIndex && queryIndex != -1
				? navigation.Uri.AsSpan()[navigation.BaseUri.Length..queryIndex]
				: navigation.Uri.AsSpan()[navigation.BaseUri.Length..];

		/// <summary>
		/// Returns if current absolute URI ends with <paramref name="suffix"/>.
		/// </summary>
		public bool IsUriEndsWith(ReadOnlySpan<char> suffix)
			=> navigation.Uri.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);

		/// <summary>
		/// Returns if current URI path ends with <paramref name="suffix"/>.
		/// </summary>
		public bool IsPathEndsWith(ReadOnlySpan<char> suffix)
			=> navigation.Path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);

		/// <summary>
		/// Return URI with specified <paramref name="uri"/> and query <paramref name="parameters"/>.
		/// </summary>
		/// <param name="uri">The destination URI. This can be absolute, or relative to the base URI (as returned by <see cref="P:NavigationManager.BaseUri" />).</param>
		/// <param name="parameters">New query parameters.</param>
		/// <param name="preserveQuery">If <see langword="true"/>, preserves current query parameters.</param>
		public string GetUri(
			[StringSyntax(StringSyntaxAttribute.Uri)] string uri,
			IReadOnlyDictionary<string, object?>? parameters = null,
			bool relativeToCurrent = false,
			bool preserveQuery = false)
		{
			if (relativeToCurrent)
				uri = UriHelper.Combine(navigation.Uri, uri);

			NavParams query = [];
			if (preserveQuery)
			{
				var queryEnumerable = new QueryStringEnumerable(QueryHelpers.GetFromUri(navigation.Uri));
				foreach (var pair in queryEnumerable)
				{
					var decodedName = pair.DecodeName();
					var decodedValue = pair.DecodeValue();
					query[decodedName.ToString()] = decodedValue.ToString();
				}
			}
			if (parameters != null)
			{
				foreach (var kv in parameters)
					query[kv.Key] = ConvertParameterValue(kv.Value);
			}
			return navigation.GetUriWithQueryParameters(uri, query);
		}

		/// <summary>
		/// Navigates to the specified <paramref name="uri"/> and query <paramref name="parameters"/>.
		/// </summary>
		/// <param name="uri">The destination URI. This can be absolute, or relative to the base URI (as returned by <see cref="NavigationManager.BaseUri"/>).</param>
		/// <param name="parameters">New query parameters.</param>
		/// <param name="forceLoad">
		/// If <see langword="true"/>, bypasses client-side routing and forces the browser to load the new page from the server,
		/// whether the URI would normally be handled by the client-side router.
		/// </param>
		/// <param name="preserveQuery">If <see langword="true"/>, preserves current query parameters.</param>
		public void NavigateTo(
			[StringSyntax(StringSyntaxAttribute.Uri)] string uri,
			IReadOnlyDictionary<string, object?>? parameters = null,
			bool forceLoad = false,
			bool relativeToCurrent = false,
			bool preserveQuery = false)
		{
			var uri2 = navigation.GetUri(uri, parameters, relativeToCurrent, preserveQuery);
			navigation.NavigateTo(uri2, forceLoad);
		}

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
		_ when value.GetType().IsEnum => ((int)value).ToString(CultureInfo.InvariantCulture),
		IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
		_ => value.ToString()
	};
}