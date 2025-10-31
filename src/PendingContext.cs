namespace UniBlazor;

/// <summary>
/// Holds metadata for pending operations.
/// </summary>
public sealed class PendingContext
{
	readonly HashSet<object> _pendings = [];

	/// <summary>
	/// An event that is raised when pending state changes.
	/// </summary>
	public event EventHandler<PendingChangedEventArgs>? OnPendingChanged;

	/// <summary>
	/// Gets a value indicating whether there are any pending operations.
	/// </summary>
	public bool IsPending()
		=> _pendings.Count != 0;

	/// <summary>
	/// Gets a value indicating whether operation for the specified key is pending.
	/// </summary>
	/// <param name="key">Operation key used in <see cref="SetPending"/>.</param>
	public bool IsPending(object key)
		=> _pendings.Contains(key);

	/// <summary>
	/// Sets or clears pending state for the specified key.
	/// </summary>
	/// <param name="key">Operation key. This can be any object compared by <see cref="Object.GetHashCode"/>.</param>
	/// <param name="value">Pending state.</param>
	public void SetPending(object key, bool value = true)
	{
		if (value)
		{
			if (_pendings.Add(key) && _pendings.Count == 1)
				OnPendingChanged?.Invoke(this, new(true));
		}
		else if (_pendings.Remove(key) && _pendings.Count == 0)
			OnPendingChanged?.Invoke(this, new(false));
	}
}

/// <summary>
/// Provides information about the <see cref="PendingContext.OnPendingChanged"/> event.
/// </summary>
/// <param name="pending">Pending state</param>
public sealed class PendingChangedEventArgs(bool pending) : EventArgs
{
	/// <summary>
	/// Gets a value indicating whether there are any pending operations.
	/// </summary>
	public bool IsPending { get; } = pending;
}