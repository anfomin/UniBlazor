using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace UniBlazor;

public static class JsExtensions
{
	/// <summary>
	/// Disposes the <see cref="IJSObjectReference"/> safely catching <see cref="JSDisconnectedException"/> .
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

	extension(IJSRuntime js)
	{
		/// <summary>
		/// Loads JS module into browser runtime.
		/// </summary>
		/// <param name="modulePath">JS-module file path.</param>
		public ValueTask<IJSObjectReference> ImportAsync(string modulePath, CancellationToken cancellationToken = default)
			=> js.InvokeAsync<IJSObjectReference>("import", cancellationToken, modulePath);

		/// <summary>
		/// Loads UniBlazor internal JS module into browser runtime.
		/// </summary>
		internal ValueTask<IJSObjectReference> ImportInternalModuleAsync(CancellationToken cancellationToken = default)
			=> js.ImportAsync("/_content/UniBlazor/Internal.js", cancellationToken);

		/// <summary>
		/// Gets cookie value by key.
		/// </summary>
		/// <param name="key">Cookie key.</param>
		/// <param name="value">Cookie value.</param>
		public async ValueTask SetCookieAsync(string key, string value, CancellationToken cancellationToken = default)
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
		public async ValueTask SetCookieAsync(string key, string value, CookieOptions options, CancellationToken cancellationToken = default)
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
		public ValueTask DownloadFileAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
			=> js.DownloadFileAsync(fileName, stream, false, cancellationToken);

		/// <summary>
		/// Downloads file to the client browser.
		/// </summary>
		/// <param name="fileName">File download name.</param>
		/// <param name="stream">File data stream.</param>
		/// <param name="leaveOpen">A flag that indicates whether the stream should be left open after transmission.</param>
		public async ValueTask DownloadFileAsync(string fileName, Stream stream, bool leaveOpen, CancellationToken cancellationToken = default)
		{
			await using var internalModule = await js.ImportInternalModuleAsync(cancellationToken);
			using var streamRef = new DotNetStreamReference(stream, leaveOpen);
			await internalModule.InvokeVoidAsync("downloadFile", cancellationToken, fileName, streamRef);
		}
	}
}