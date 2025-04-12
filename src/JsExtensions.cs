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
	/// Downloads file to the client browser.
	/// </summary>
	/// <param name="fileName">File download name.</param>
	/// <param name="stream">File data stream.</param>
	/// <param name="leaveOpen">A flag that indicates whether the stream should be left open after transmission.</param>
	public static async ValueTask DownloadFileAsync(this IJSRuntime js, string fileName, Stream stream, bool leaveOpen = false)
	{
		using var streamRef = new DotNetStreamReference(stream, leaveOpen);
		await js.InvokeVoidAsync("UniBlazor.downloadFile", fileName, streamRef);
	}
}