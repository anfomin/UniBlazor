using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Initializes imask.js for inputs with <c>data-mask</c> and <c>data-mask-regexp</c> attributes.
/// </summary>
public sealed class UniMask : UniJSComponentBase
{
	protected override string DisposeMethodName => "destroy";

	protected override async Task InitializeJSAsync()
	{
		await using var module = await JS.ImportAsync("/_content/UniBlazor/UniMask.js");
		JSObject = await module.InvokeAsync<IJSObjectReference>("create", Aborted);
	}
}