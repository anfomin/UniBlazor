using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace UniBlazor;

public sealed partial class UniAudio : UniJSComponentBase
{
	ElementReference? _audioRef;

	/// <summary>
	/// Gets or sets the source URL of the audio file.
	/// </summary>
	[Parameter, EditorRequired]
	public string? Src { get; set; }

	/// <summary>
	/// Gets or sets whether the audio should start playing automatically when loaded.
	/// </summary>
	[Parameter]
	public bool AutoPlay { get; set; }

	/// <summary>
	/// Gets or sets whether the audio should loop when it reaches the end.
	/// </summary>
	[Parameter]
	public bool Loop { get; set; }

	/// <summary>
	/// Gets or sets whether the audio controls should be displayed.
	/// Default is <c>true</c>.
	/// </summary>
	[Parameter]
	public bool Controls { get; set; } = true;

	/// <summary>
	/// Gets or sets preload behavior.
	/// </summary>
	[Parameter]
	public AudioPreload Preload { get; set; }

	/// <summary>
	/// Specifies the content to be rendered when audio element is not supported.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Gets current audio status.
	/// </summary>
	public AudioStatus Status { get; private set; }

	/// <summary>
	/// Occurs when the audio status changes.
	/// </summary>
	[Parameter]
	public EventCallback<AudioStatus> OnStatus { get; set; }

	protected override async Task InitializeJSAsync()
	{
		await base.InitializeJSAsync();
		JSObject = await JS.ImportInternalModuleAsync(Aborted);
	}

	async Task SetStatusAsync(AudioStatus status)
	{
		Status = status;
		await OnStatus.InvokeAsync(status);
	}

	/// <summary>
	/// Invokes play on the audio element.
	/// </summary>
	/// <returns><c>true</c> if play method invoked. <c>false</c> if JS runtime or audio element is not available.</returns>
	public async ValueTask<bool> PlayAsync(CancellationToken cancellationToken = default)
	{
		if (JSObject is null || _audioRef is not { } audio)
			return false;
		await JSObject.InvokeVoidAsync("invokeElement", cancellationToken, audio, "play");
		return true;
	}

	/// <summary>
	/// Invokes pause on the audio element.
	/// </summary>
	/// <returns><c>true</c> if pause method invoked. <c>false</c> if JS runtime or audio element is not available.</returns>
	public async ValueTask<bool> PauseAsync(CancellationToken cancellationToken = default)
	{
		if (JSObject is null || _audioRef is not { } audio)
			return false;
		await JSObject.InvokeVoidAsync("invokeElement", cancellationToken, audio, "pause");
		return true;
	}

	/// <summary>
	/// Toggles play/pause on the audio element.
	/// </summary>
	/// <returns><c>true</c> if play/pause method invoked. <c>false</c> if JS runtime or audio element is not available.</returns>
	public async ValueTask<bool> PlayPauseAsync(CancellationToken cancellationToken = default)
	{
		if (JSObject is null || _audioRef is not { } audio)
			return false;
		await JSObject.InvokeVoidAsync("invokeElement", cancellationToken, audio, Status == AudioStatus.Playing ? "pause" : "play");
		return true;
	}
}