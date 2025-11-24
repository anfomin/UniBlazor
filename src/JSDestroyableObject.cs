using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Represents object holding <see cref="IJSObjectReference"/> and invokes JS <c>destroy()</c> method on dispose.
/// </summary>
public class JSDestroyableObject : JSObject
{
	/// <summary>
	/// Initializes an empty instance of the <see cref="JSDestroyableObject"/> class.
	/// <see cref="JSObject.Ref"/> should be set later.
	/// </summary>
	public JSDestroyableObject() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="JSDestroyableObject"/> class.
	/// </summary>
	/// <param name="reference">JavaScript object reference.</param>
	public JSDestroyableObject(IJSObjectReference reference) : base(reference) { }

	protected override async ValueTask DisposeAsyncCore()
	{
		try
		{
			if (Ref is not null)
				await Ref.InvokeVoidAsync("destroy");
		}
		catch (JSDisconnectedException) { }
		catch (OperationCanceledException) { }
		finally
		{
			await base.DisposeAsyncCore();
		}
	}
}