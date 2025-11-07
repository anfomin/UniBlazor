using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Initializes imask.js for inputs with 'data-mask' and 'data-mask-regexp' attributes.
/// </summary>
public sealed partial class UniMask : ComponentBase, IAsyncDisposable
{
	JsObject? _jsMask;

	/// <summary>
	/// Gets logger.
	/// </summary>
	[Inject]
	ILogger<UniMask> Logger { get; set; } = null!;

	/// <summary>
	/// Gets JS runtime.
	/// </summary>
	[Inject]
	IJSRuntime JS { get; set; } = null!;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			try
			{
				await using var module = await JS.ImportAsync("/_content/UniBlazor/UniMask.js");
				_jsMask = new(await module.InvokeAsync<IJSObjectReference>("create"));
			}
			catch (JSDisconnectedException) { }
			catch (JSException ex) when (ex.Message == "null") { }
		}
	}

	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		if (_jsMask != null)
			await _jsMask.DisposeAsync();
	}
}