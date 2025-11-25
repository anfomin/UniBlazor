using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Base class for JavaScript interop components holding <see cref="IJSObjectReference"/> and safely disposing it.
/// Supports invoking JS <see cref="DisposeMethodName"/> before disposing the reference.
/// </summary>
public abstract class UniJSComponentBase : UniComponentBase
{
	/// <summary>
	/// Gets the JavaScript runtime.
	/// </summary>
	[Inject]
	protected IJSRuntime JS { get; private set; } = default!;

	/// <summary>
	/// Gets or sets the JavaScript object reference.
	/// </summary>
	protected IJSObjectReference? JSObject { get; set; }

	/// <summary>
	/// Gets the name of the JavaScript method to call on <see cref="JSObject"/> before disposing the reference.
	/// </summary>
	protected virtual string? DisposeMethodName => null;

	protected override async ValueTask DisposeAsyncCore()
	{
		if (JSObject is not null)
		{
			if (DisposeMethodName is { } method)
			{
				try
				{
					await JSObject.InvokeVoidAsync(method);
				}
				catch (JSDisconnectedException) { }
				catch (OperationCanceledException) { }
			}
			await JSObject.DisposeAsyncSafe();
			JSObject = null;
		}
		await base.DisposeAsyncCore();
	}
}