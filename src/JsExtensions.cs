using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Provides extensions for <see cref="IJSRuntime"/>.
/// </summary>
public static class JsExtensions
{
	/// <summary>
	/// Loads js-module into browser runtime.
	/// </summary>
	/// <param name="modulePath">JS-module file path.</param>
	public static ValueTask<IJSObjectReference> ImportAsync(this IJSRuntime js, string modulePath, CancellationToken cancellationToken = default)
		=> js.InvokeAsync<IJSObjectReference>("import", cancellationToken, modulePath);

	/// <summary>
	/// Loads UniBlazor internal js-module into browser runtime.
	/// </summary>
	internal static ValueTask<IJSObjectReference> ImportInternalModuleAsync(this IJSRuntime js, CancellationToken cancellationToken = default)
		=> js.ImportAsync("/_content/UniBlazor/Internal.js", cancellationToken);

	/// <summary>
	/// Disposes the JS object reference safely catching <see cref="JSDisconnectedException"/> .
	/// </summary>
	public static async ValueTask DisposeAsyncSafe(this IJSObjectReference jsRef)
	{
		try
		{
			await jsRef.DisposeAsync();
		}
		catch (JSDisconnectedException) { }
		catch (OperationCanceledException) { }
	}

	/// <summary>
	/// Gets cookie value by key.
	/// </summary>
	/// <param name="key">Cookie key.</param>
	/// <param name="value">Cookie value.</param>
	public static async ValueTask SetCookieAsync(this IJSRuntime js, string key, string value, CancellationToken cancellationToken = default)
	{
		await using var internalModule = await js.ImportInternalModuleAsync(cancellationToken);
		await internalModule.InvokeVoidAsync("setCookie", cancellationToken, key, value);
	}

	/// <summary>
	/// Sets cookie value by key with options.
	/// </summary>
	/// <param name="key">Cookie key.</param>
	/// <param name="value">Cookie value.</param>
	/// <param name="options">Cookie options.</param>
	public static async ValueTask SetCookieAsync(this IJSRuntime js, string key, string value, CookieOptions options, CancellationToken cancellationToken = default)
	{
		await using var internalModule = await js.ImportInternalModuleAsync(cancellationToken);
		await internalModule.InvokeVoidAsync("setCookie", cancellationToken, key, value, new
		{
			options.Domain,
			options.Path,
			Expires = options.Expires?.ToString("r"),
			MaxAge = (int?)options.MaxAge?.TotalSeconds,
			SameSite = options.SameSite == SameSiteMode.Unspecified ? null : options.SameSite.ToString().ToLower(),
			options.Secure
		});
	}

	/// <summary>
	/// Downloads file to the client browser.
	/// Stream will be closed after transmission.
	/// </summary>
	/// <param name="fileName">File download name.</param>
	/// <param name="stream">File data stream.</param>
	public static ValueTask DownloadFileAsync(this IJSRuntime js, string fileName, Stream stream, CancellationToken cancellationToken = default)
		=> js.DownloadFileAsync(fileName, stream, false, cancellationToken);

	/// <summary>
	/// Downloads file to the client browser.
	/// </summary>
	/// <param name="fileName">File download name.</param>
	/// <param name="stream">File data stream.</param>
	/// <param name="leaveOpen">A flag that indicates whether the stream should be left open after transmission.</param>
	public static async ValueTask DownloadFileAsync(this IJSRuntime js, string fileName, Stream stream, bool leaveOpen, CancellationToken cancellationToken = default)
	{
		await using var internalModule = await js.ImportInternalModuleAsync(cancellationToken);
		using var streamRef = new DotNetStreamReference(stream, leaveOpen);
		await internalModule.InvokeVoidAsync("downloadFile", cancellationToken, fileName, streamRef);
	}
}