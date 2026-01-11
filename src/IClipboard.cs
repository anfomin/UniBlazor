namespace UniBlazor;

/// <summary>
/// Provides clipboard interaction.
/// </summary>
public interface IClipboard
{
	/// <summary>
	/// Detects if clipboard interaction is supported.
	/// </summary>
	ValueTask<bool> IsSupportedAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Reads text from the clipboard.
	/// </summary>
	ValueTask<string> ReadTextAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Writes text to the clipboard.
	/// </summary>
	/// <param name="text">Text to write to clipboard.</param>
	ValueTask WriteTextAsync(string text, CancellationToken cancellationToken = default);
}