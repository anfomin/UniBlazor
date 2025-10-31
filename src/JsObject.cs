using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Represents a JavaScript object reference that invokes <c>dispose()</c> method on dispose.
/// </summary>
/// <param name="jsRef">JavaScript object reference.</param>
public class JsObject(IJSObjectReference jsRef) : IAsyncDisposable
{
	/// <summary>
	/// Gets the JavaScript object reference.
	/// </summary>
	public IJSObjectReference JsRef { get; } = jsRef;

	/// <summary>
	/// Gets a value determining if the component and associated services have been disposed.
	/// </summary>
	protected bool IsDisposed { get; private set; }

	/// <inheritdoc />
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
		try
		{
			await JsRef.InvokeVoidAsync("dispose");
		}
		catch (JSDisconnectedException) { }
		catch (OperationCanceledException) { }
		finally
		{
			await JsRef.DisposeAsyncSafe();
		}
	}
}