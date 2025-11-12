using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Represents object holding <see cref="IJSObjectReference"/> and invokes JS <c>destroy()</c> method on dispose.
/// </summary>
/// <param name="jsRef">JavaScript object reference.</param>
public class JSDestroyableObject(IJSObjectReference jsRef) : JSObject(jsRef)
{
	protected override async ValueTask DisposeAsyncCore()
	{
		try
		{
			await JsRef.InvokeVoidAsync("destroy");
		}
		catch (JSDisconnectedException) { }
		catch (OperationCanceledException) { }
		finally
		{
			await base.DisposeAsyncCore();
		}
	}
}