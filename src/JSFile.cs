using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Represents a file uploaded from browser via JavaScript interop.
/// </summary>
/// <param name="name">The name of the file as specified by the browser.</param>
/// <param name="lastModified">The last modified date as specified by the browser.</param>
/// <param name="contentType">The MIME type of the file as specified by the browser.</param>
/// <param name="stream">JS file stream.</param>
public sealed class JSFile(
	string name,
	long lastModified,
	string contentType,
	IJSStreamReference stream)
	: IBrowserFileAsync
{
	IJSStreamReference? _stream = stream;

	public string Name { get; } = name;
	public long Size { get; } = stream.Length;
	public DateTimeOffset LastModified { get; } = DateTimeOffset.FromUnixTimeMilliseconds(lastModified);
	public string ContentType { get; } = contentType;

	public async ValueTask DisposeAsync()
	{
		if (_stream != null)
		{
			await _stream.DisposeAsync();
			_stream = null;
		}
	}

	/// <summary>
	/// Throws <see cref="NotSupportedException"/>. Use <see cref="OpenReadStreamAsync(long, CancellationToken)"/> instead.
	/// </summary>
	public Stream OpenReadStream(long maxAllowedSize, CancellationToken cancellationToken = default)
		=> throw new NotSupportedException("Synchronous stream opening is not supported. Use OpenReadStreamAsync instead.");

	public ValueTask<Stream> OpenReadStreamAsync(long maxAllowedSize, CancellationToken cancellationToken = default)
	{
		if (_stream == null)
			throw new InvalidOperationException("File was disposed");
		return _stream.OpenReadStreamAsync(maxAllowedSize, cancellationToken);
	}
}