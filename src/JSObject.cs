using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Represents object holding <see cref="IJSObjectReference"/> and safely disposing it.
/// Supports invoking JS <see cref="DisposeMethodName"/> before disposing the reference.
/// </summary>
/// <param name="reference">JavaScript object reference.</param>
public class JSObject(IJSObjectReference reference) : IAsyncDisposable
{
	/// <summary>
	/// Gets the JavaScript object reference.
	/// </summary>
	public IJSObjectReference Ref { get; } = reference;

	/// <summary>
	/// Gets a value determining if the component and associated services have been disposed.
	/// </summary>
	protected bool IsDisposed { get; private set; }

	/// <summary>
	/// Gets the name of the JavaScript method to call before disposing the reference.
	/// </summary>
	protected virtual string? DisposeMethodName => null;

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
		if (DisposeMethodName is { } method)
		{
			try
			{
				await Ref.InvokeVoidAsync(method);
			}
			catch (JSDisconnectedException) { }
			catch (OperationCanceledException) { }
		}
		await Ref.DisposeAsyncSafe();
	}
}