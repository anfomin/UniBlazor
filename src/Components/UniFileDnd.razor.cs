using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// A drag &amp; drop file upload component.
/// </summary>
public sealed partial class UniFileDnd : UniComponentBase
{
	readonly List<WeakReference<JSFile>> _files = [];
	ElementReference _elementRef;
	IJSObjectReference? _jsModule;
	DotNetObjectReference<Handler>? _jsHandler;

	/// <summary>
	/// Gets JS runtime.
	/// </summary>
	[Inject]
	IJSRuntime JS { get; set; } = null!;

	/// <summary>
	/// Specifies container CSS class.
	/// </summary>
	[Parameter]
	public string? Class { get; set; }

	/// <summary>
	/// Specifies a comma-separated list of one or more file types,
	/// or unique file type specifiers, describing which file types to allow.
	/// Should be set before component initialization.
	/// </summary>
	[Parameter]
	public string? Accept { get; set; }

	/// <summary>
	/// Specifies whether the file drag &amp; drop is disabled.
	/// </summary>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary>
	/// Specifies the content to be rendered inside this component.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// A callback that will be invoked when a file is selected.
	/// </summary>
	[Parameter]
	public EventCallback<IBrowserFileAsync> OnFile { get; set; }

	protected override async Task InitializeJSAsync()
	{
		try
		{
			_jsModule = await JS.ImportAsync("/_content/UniBlazor/UniFileDnd.js");
			_jsHandler = DotNetObjectReference.Create(new Handler(this));
			await _jsModule.InvokeVoidAsync("init", _elementRef, _jsHandler);
		}
		catch (JSDisconnectedException) { }
	}

	protected override async ValueTask DisposeAsyncCore()
	{
		await base.DisposeAsyncCore();
		if (_jsModule is not null)
		{
			await _jsModule.DisposeAsyncSafe();
			_jsModule = null;
		}
		if (_jsHandler is not null)
		{
			_jsHandler.Dispose();
			_jsHandler = null;
		}
		foreach (var fileRef in _files)
		{
			if (fileRef.TryGetTarget(out var file))
				await file.DisposeAsync();
		}
	}

	/// <summary>
	/// Opens the file picker dialog.
	/// </summary>
	public async Task OpenPickerAsync(CancellationToken cancellationToken = default)
	{
		if (Disabled)
			return;
		if (_jsModule is null || _jsHandler is null)
			throw new InvalidOperationException("JS drag & drop module is disposed");
		await _jsModule.InvokeVoidAsync("openPicker", cancellationToken, _jsHandler, Accept);
	}

	class Handler(UniFileDnd component)
	{
		[JSInvokable]
		public async Task SelectFileAsync(string name, long lastModified, string contentType, IJSStreamReference stream)
		{
			if (component.Disabled)
				return;
			var file = new JSFile(name, lastModified, contentType, stream);
			component._files.Add(new(file));
			await component.OnFile.InvokeAsync(file);
		}
	}
}