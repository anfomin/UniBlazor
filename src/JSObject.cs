using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Represents object holding <see cref="IJSObjectReference"/> and safely disposing it.
/// </summary>
public class JSObject : IAsyncDisposable
{
	/// <summary>
	/// Gets the JavaScript object reference.
	/// </summary>
	public IJSObjectReference? Ref { get; protected set; }

	/// <summary>
	/// Gets a value determining if the component and associated services have been disposed.
	/// </summary>
	protected bool IsDisposed { get; private set; }

	/// <summary>
	/// Initializes an empty instance of the <see cref="JSObject"/> class.
	/// <see cref="Ref"/> should be set later.
	/// </summary>
	public JSObject() { }

	/// <summary>
	/// Initializes a new instance of the <see cref="JSObject"/> class.
	/// </summary>
	/// <param name="reference">JavaScript object reference.</param>
	public JSObject(IJSObjectReference reference)
		=> Ref = reference;

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
		if (Ref is not null)
			await Ref.DisposeAsyncSafe();
	}

	/// <summary>
	/// Ensures that the JavaScript object reference has not been created yet.
	/// </summary>
	protected void EnsureNoRef()
	{
		if (Ref is not null)
			throw new InvalidOperationException("JavaScript object reference has already been created.");
	}

	/// <summary>
	/// Ensures that the JavaScript object reference has been created and not disposed.
	/// </summary>
	[MemberNotNull(nameof(Ref))]
	public void EnsureRef()
	{
		if (Ref is null)
			throw new InvalidOperationException("JavaScript object reference has not been created yet.");
		if (IsDisposed)
			throw new InvalidOperationException("JavaScript object reference has been disposed.");
	}
}