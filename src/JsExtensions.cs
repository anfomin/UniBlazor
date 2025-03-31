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
}