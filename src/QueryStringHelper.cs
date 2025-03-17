namespace UniBlazor;

/// <summary>
/// Provides helper methods for working with query strings.
/// </summary>
public static class QueryStringHelper
{
	/// <summary>
	/// Returns query string from the specified URL.
	/// </summary>
	public static ReadOnlyMemory<char> GetQueryString(string? url)
	{
		if (url is null)
			return default;

		var queryStartPos = url.IndexOf('?');
		if (queryStartPos < 0)
			return default;

		var queryEndPos = url.IndexOf('#', queryStartPos);
		return url.AsMemory(queryStartPos..(queryEndPos < 0 ? url.Length : queryEndPos));
	}
}