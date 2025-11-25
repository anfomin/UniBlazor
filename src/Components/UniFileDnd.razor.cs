using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// A drag &amp; drop file upload component.
/// </summary>
public sealed partial class UniFileDnd : UniJSComponentBase
{
	readonly List<WeakReference<JSFile>> _files = [];
	ElementReference _elementRef;
	DotNetObjectReference<Handler>? _jsHandler;

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
			JSObject = await JS.ImportAsync("/_content/UniBlazor/UniFileDnd.js");
			_jsHandler = DotNetObjectReference.Create(new Handler(this));
			await JSObject.InvokeVoidAsync("init", _elementRef, _jsHandler);
		}
		catch (JSDisconnectedException) { }
	}

	protected override async ValueTask DisposeAsyncCore()
	{
		await base.DisposeAsyncCore();
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
		if (JSObject is null || _jsHandler is null)
			throw new InvalidOperationException("JS drag & drop module is disposed");
		await JSObject.InvokeVoidAsync("openPicker", cancellationToken, _jsHandler, Accept);
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