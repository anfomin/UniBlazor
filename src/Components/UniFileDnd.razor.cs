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
	/// Specifies whether multiple file selection is allowed.
	/// </summary>
	[Parameter]
	public bool Multiple { get; set; }

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
	public EventCallback<File> OnFile { get; set; }

	protected override async Task InitializeJSAsync()
	{
		try
		{
			JSObject = await JS.ImportAsync("/_content/UniBlazor/UniFileDnd.js");
			_jsHandler = DotNetObjectReference.Create(new Handler(this));
			await JSObject.InvokeVoidAsync("init", _elementRef, _jsHandler, Multiple);
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
		await JSObject.InvokeVoidAsync("openPicker", cancellationToken, _jsHandler, Accept, Multiple);
	}

	class Handler(UniFileDnd component)
	{
		[JSInvokable]
		public async Task<bool> SelectFileAsync(int index, int total, string name, long lastModified, string contentType, IJSStreamReference stream)
		{
			if (component.Disabled)
				return false;
			var file = new File(name, lastModified, contentType, stream)
			{
				Index = index,
				Total = total
			};
			component._files.Add(new(file));
			await component.OnFile.InvokeAsync(file);
			return !file.Canceled;
		}
	}

	/// <summary>
	/// Represents a file selected via the <see cref="UniFileDnd"/> component.
	/// </summary>
	/// <param name="name">The name of the file as specified by the browser.</param>
	/// <param name="lastModified">The last modified date as specified by the browser.</param>
	/// <param name="contentType">The MIME type of the file as specified by the browser.</param>
	/// <param name="stream">JS file stream.</param>
	public class File(string name, long lastModified, string contentType, IJSStreamReference stream)
		: JSFile(name, lastModified, contentType, stream)
	{
		/// <summary>
		/// Gets file index for multiple selection.
		/// </summary>
		public required int Index { get; init; }

		/// <summary>
		/// Gets total number of selected files for multiple selection.
		/// </summary>
		public required int Total { get; init; }

		/// <summary>
		/// Gets whether the file upload was canceled.
		/// </summary>
		public bool Canceled { get; private set; }

		/// <summary>
		/// Cancels next files upload in multiple selection.
		/// </summary>
		public void Cancel()
			=> Canceled = true;
	}
}