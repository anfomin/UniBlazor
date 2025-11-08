namespace UniBlazor;

/// <summary>
/// Specifies the current status of the audio playback.
/// </summary>
public enum AudioStatus
{
	/// <summary>
	/// Indicates that the audio is stopped.
	/// </summary>
	Stopped,

	/// <summary>
	/// Indicates that the audio is loading.
	/// </summary>
	Loading,

	/// <summary>
	/// Indicates that the audio is currently playing.
	/// </summary>
	Playing,

	/// <summary>
	/// Indicates that the audio is paused.
	/// </summary>
	Paused,

	/// <summary>
	/// Indicates that an error occurred during audio playback.
	/// </summary>
	Error
}