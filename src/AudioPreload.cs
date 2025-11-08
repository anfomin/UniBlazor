namespace UniBlazor;

/// <summary>
/// Specifies how the audio element should preload the audio data.
/// </summary>
public enum AudioPreload
{
	/// <summary>
	/// Indicates that the whole audio file can be downloaded, even if the user is not expected to use it.
	/// </summary>
	Auto,

	/// <summary>
	/// Indicates that only audio metadata (e.g., length) is fetched.
	/// </summary>
	Metadata,

	/// <summary>
	///Indicates that the audio should not be preloaded.
	/// </summary>
	None
}