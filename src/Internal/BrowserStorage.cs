using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace UniBlazor.Internal;

/// <summary>
/// Represents <see cref="IStorageBase"/> implementation for browser via <see cref="IJSRuntime"/>.
/// </summary>
public abstract class BrowserStorage(IJSRuntime js, IOptions<JsonOptions> jsonOptions, string storageName) : IStorageBase
{
	readonly IJSRuntime _js = js;
	readonly JsonOptions _jsonOptions = jsonOptions.Value;
	readonly string _storageName = storageName;

	/// <inheritdoc/>
	public ValueTask ClearAsync(CancellationToken cancellationToken = default)
		=> _js.InvokeVoidAsync($"{_storageName}.clear", cancellationToken);

	/// <inheritdoc/>
	public ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
		=> _js.InvokeAsync<int>("eval", cancellationToken, $"{_storageName}.length");

	/// <inheritdoc/>
	public ValueTask<string?> GetAsync(string key, CancellationToken cancellationToken = default)
		=> _js.InvokeAsync<string?>($"{_storageName}.getItem", cancellationToken, key);

	/// <inheritdoc/>
	public async ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
		where T : class
		=> await GetAsync(key, cancellationToken) is { } json
			? JsonSerializer.Deserialize<T>(json, _jsonOptions.SerializerOptions)
			: default;

	/// <inheritdoc/>
	public async ValueTask<T?> GetOrNullAsync<T>(string key, CancellationToken cancellationToken = default)
		where T : struct
		=> await GetAsync(key, cancellationToken) is { } json
			? JsonSerializer.Deserialize<T>(json, _jsonOptions.SerializerOptions)
			: null;

	/// <inheritdoc/>
	public ValueTask SetAsync(string key, string value, CancellationToken cancellationToken = default)
		=> _js.InvokeVoidAsync($"{_storageName}.setItem", cancellationToken, key, value);

	/// <inheritdoc/>
	public ValueTask SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
	{
		string json = JsonSerializer.Serialize(value, _jsonOptions.SerializerOptions);
		return SetAsync(key, json, cancellationToken);
	}

	/// <inheritdoc/>
	public ValueTask<string?> KeyAtAsync(int index, CancellationToken cancellationToken = default)
		=> _js.InvokeAsync<string?>($"{_storageName}.key", cancellationToken, index);

	/// <inheritdoc/>
	public ValueTask<ICollection<string>> KeysAsync(CancellationToken cancellationToken = default)
		=> _js.InvokeAsync<ICollection<string>>("eval", cancellationToken, $"Object.keys({_storageName})");

	/// <inheritdoc/>
	public ValueTask<bool> ContainKeyAsync(string key, CancellationToken cancellationToken = default)
		=> _js.InvokeAsync<bool>($"{_storageName}.hasOwnProperty", cancellationToken, key);

	/// <inheritdoc/>
	public ValueTask RemoveAsync(string key, CancellationToken cancellationToken = default)
		=> _js.InvokeVoidAsync($"{_storageName}.removeItem", cancellationToken, key);
}