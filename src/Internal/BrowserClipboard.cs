using Microsoft.JSInterop;

namespace UniBlazor.Internal;

/// <summary>
/// Represents <see cref="IClipboard"/> implementation for browser via <see cref="IJSRuntime"/>.
/// </summary>
public sealed class BrowserClipboard(IJSRuntime js) : IClipboard
{
	readonly IJSRuntime _js = js;

	public ValueTask<bool> IsSupportedAsync(CancellationToken cancellationToken = default)
		=> _js.InvokeAsync<bool>("eval", cancellationToken, "!!navigator.clipboard && !!navigator.clipboard.readText");

	public ValueTask<string> ReadTextAsync(CancellationToken cancellationToken = default)
		=> _js.InvokeAsync<string>("navigator.clipboard.readText", cancellationToken);

	public ValueTask WriteTextAsync(string text, CancellationToken cancellationToken = default)
		=> _js.InvokeVoidAsync("navigator.clipboard.writeText", cancellationToken, text);
}