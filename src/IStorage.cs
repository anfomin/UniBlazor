namespace UniBlazor;

/// <summary>
/// Abstract key-value storage.
/// </summary>
public interface IStorageBase
{
	/// <summary>
	/// Clears all items from the storage.
	/// </summary>
	ValueTask ClearAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Returns number of items stored in the storage.
	/// </summary>
	ValueTask<int> CountAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves <see cref="string"/> value by the specified <paramref name="key"/> from the storage.
	/// </summary>
	/// <param name="key">Storage key to get value for.</param>
	/// <returns><see cref="String"/> value or <c>null</c> if key does not exist in the storage.</returns>
	ValueTask<string?> GetAsync(string key, CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves value by the specified <paramref name="key"/> from the storage as JSON-string and deserializes it to the <typeparamref name="T"/>.
	/// </summary>
	/// <param name="key">Storage key to get value for.</param>
	/// <typeparam name="T">Type to deserialize JSON-string to.</typeparam>
	/// <returns><typeparamref name="T"/> value or <c>null</c> if key does not exist in the storage.</returns>
	ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
		where T : class;

	/// <summary>
	/// Retrieves value by the specified <paramref name="key"/> from the storage as JSON-string and deserializes it to the <typeparamref name="T"/>.
	/// </summary>
	/// <param name="key">Storage key to get value for.</param>
	/// <typeparam name="T">Type to deserialize JSON-string to.</typeparam>
	/// <returns><typeparamref name="T"/> value or <c>null</c> if key does not exist in the storage.</returns>
	ValueTask<T?> GetOrNullAsync<T>(string key, CancellationToken cancellationToken = default)
		where T : struct;

	/// <summary>
	/// Sets or updates the <see cref="string"/> <paramref name="value"/> in the storage by the specified <paramref name="key"/>.
	/// </summary>
	/// <param name="key">Storage key to set value for.</param>
	/// <param name="value"><see cref="String"/> value to set.</param>
	ValueTask SetAsync(string key, string value, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sets or updates the JSON-serialized <paramref name="value"/> in the storage by the specified <paramref name="key"/>.
	/// </summary>
	/// <param name="key">Storage key to set value for.</param>
	/// <param name="value"><typeparamref name="T"/> value to serialize to JSON.</param>
	ValueTask SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns key at the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">Storage index to get key for.</param>
    /// <returns><see cref="String"/> key or <c>null</c> if key does not exist at specified index.</returns>
    ValueTask<string?> KeyAtAsync(int index, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a collection of strings representing the keys in the storage.
    /// </summary>
    ValueTask<ICollection<string>> KeysAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns if the <paramref name="key"/> exists in the storage, but does not check its value.
    /// </summary>
	/// <param name="key">Storage key to check.</param>
    /// <returns><c>True</c> if storage contains key. Otherwise, <c>false</c>.</returns>
    ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the specified <paramref name="key"/> from the storage.
    /// </summary>
	/// <param name="key">Storage key to remove.</param>
    ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default);

	/// <summary>
	/// Removes a collection of <paramref name="keys"/> from the storage.
	/// </summary>
	/// <param name="keys">Storage keys to remove.</param>
	async ValueTask RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
	{
		foreach (string key in keys)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await RemoveAsync(key, cancellationToken);
		}
	}
}

/// <summary>
/// Key-value local storage.
/// </summary>
public interface ILocalStorage : IStorageBase { }

/// <summary>
/// Key-value session storage.
/// </summary>
public interface ISessionStorage : IStorageBase { }