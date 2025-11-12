using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Represents object holding <see cref="IJSObjectReference"/> and safely disposing it.
/// </summary>
/// <param name="jsRef">JavaScript object reference.</param>
public class JSObject(IJSObjectReference jsRef) : IAsyncDisposable
{
	/// <summary>
	/// Gets the JavaScript object reference.
	/// </summary>
	public IJSObjectReference JsRef { get; } = jsRef;

	/// <summary>
	/// Gets a value determining if the component and associated services have been disposed.
	/// </summary>
	protected bool IsDisposed { get; private set; }

	public async ValueTask DisposeAsync()
	{
		if (!IsDisposed)
		{
			IsDisposed = true;
			await DisposeAsyncCore().ConfigureAwait(false);
			GC.SuppressFinalize(this);
		}
	}

	/// <summary>
	/// Asynchronously disposes the object reference.
	/// </summary>
	protected virtual async ValueTask DisposeAsyncCore()
	{
		await JsRef.DisposeAsyncSafe();
	}
}