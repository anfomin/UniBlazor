using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Renders <see cref="EditForm"/> with:
/// <list type="bullet">
/// <item><see cref="DataAnnotationsValidator"/>.</item>
/// <item><see cref="ValidationSummary"/>.</item>
/// <item>Hidden submit button for enter-key submit.</item>
/// </list>
/// Provides navigation confirmation if <see cref="EditContext"/> is modified.
/// </summary>
public sealed partial class UniForm : ComponentBase, IAsyncDisposable
{
	IJSObjectReference? _jsModule;
	EditForm? _form;

	/// <summary>
	/// Gets form unique identifier.
	/// </summary>
	public string Id { get; } = Guid.NewGuid().ToString("N");

	/// <summary>
	/// Gets logger.
	/// </summary>
	[Inject]
	ILogger<UniForm> Logger { get; set; } = null!;

	/// <summary>
	/// Gets JS runtime.
	/// </summary>
	[Inject]
	IJSRuntime JS { get; set; } = null!;

	/// <summary>
	/// Specifies the top-level model object for the form. An edit context will be constructed for this model.
	/// </summary>
	[Parameter, EditorRequired]
	public object? Model { get; set; }

	/// <summary>
	/// Specifies form CSS class.
	/// </summary>
	[Parameter]
	public string? Class { get; set; }

	/// <summary>
	/// Specifies form CSS style.
	/// </summary>
	[Parameter]
	public string? Style { get; set; }

	/// <summary>
	/// Specifies CSS class for validation summary.
	/// </summary>
	public string? ValidationClass { get; set; }

	/// <summary>
	/// Specifies CSS style for validation summary.
	/// </summary>
	public string? ValidationStyle { get; set; }

	/// <summary>
	/// Specifies the content to be rendered inside this <see cref="T:Microsoft.AspNetCore.Components.Forms.EditForm" />.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// A callback that will be invoked when the form is submitted and the EditContext is determined to be valid.
	/// </summary>
	[Parameter]
	public EventCallback<EditContext> OnSubmit { get; set; }

	/// <summary>
	/// A callback when will be invoked to confirm navigation.
	/// </summary>
	[Parameter]
	public EventCallback<LocationChangingContext> OnConfirmNavigation { get; set; }

	/// <summary>
	/// Gets or sets if navigation should be confirmed.
	/// </summary>
	[Parameter]
	public bool ConfirmNavigation { get; set; } = true;

	/// <summary>
	/// Event raised before form submission.
	/// </summary>
	public event EventHandler<BeforeSubmitEventArgs>? BeforeSubmit;

	/// <summary>
	/// Gets form <see cref="EditContext"/>.
	/// </summary>
	public EditContext? EditContext => _form?.EditContext;

	/// <summary>
	/// Gets if form should confirm navigation.
	/// </summary>
	public bool ShouldConfirmNavigation => ConfirmNavigation && Model != null && _form?.EditContext?.IsModified() == true;

	/// <summary>
	/// Specifies default CSS class for every <see cref="UniForm"/>.
	/// </summary>
	public static string? DefaultClass { get; set; }

	/// <summary>
	/// Specifies default CSS style for every <see cref="UniForm"/> validation summary.
	/// </summary>
	public static string? DefaultValidationClass { get; set; }

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
			_jsModule = await JS.ImportAsync("/_content/UniBlazor/Components/UniForm.razor.js");
	}

	/// <inheritdoc />
	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		if (_jsModule != null)
			await _jsModule.DisposeAsyncSafe();
	}

	/// <summary>
	/// Validates form and invoked <see cref="OnSubmit"/> if valid.
	/// </summary>
	/// <exception cref="InvalidOperationException"><see cref="EditForm"/> or <see cref="EditContext"/> is not initialized.</exception>
	public async Task SubmitAsync()
	{
		if (_form == null)
			throw new InvalidOperationException("Form is null");

		var context = _form.EditContext ?? throw new InvalidOperationException("Form EditContext is null");
		var args = new BeforeSubmitEventArgs(context);
		BeforeSubmit?.Invoke(this, args);
		if (args.IsPrevented)
		{
			Logger.LogDebug("Submit is prevented");
			return;
		}

		if (context.Validate())
			await OnSubmit.InvokeAsync(context);
		else
			await OnInvalidAsync(context);
	}

	async Task OnInvalidAsync(EditContext context)
	{
		Logger.LogDebug("Form is invalid");
		foreach (string message in context.GetValidationMessages())
			Logger.LogDebug(message);

		if (_jsModule != null)
		{
			try
			{
				await _jsModule.InvokeVoidAsync("scrollToFirstError", Id);
			}
			catch (JSDisconnectedException) { }
		}
	}

	async Task OnBeforeNavigationAsync(LocationChangingContext context)
	{
		if (ShouldConfirmNavigation)
			await OnConfirmNavigation.InvokeAsync(context);
	}

	/// <summary>
	/// Event arguments for <see cref="BeforeSubmit"/> event.
	/// </summary>
	/// <param name="context">Current form context.</param>
	public sealed class BeforeSubmitEventArgs(EditContext context) : EventArgs
	{
		/// <summary>
		/// Gets current form <see cref="EditContext"/>.
		/// </summary>
		public EditContext EditContext { get; } = context;

		/// <summary>
		/// Gets if submit event will be prevented.
		/// </summary>
		public bool IsPrevented { get; private set; }

		/// <summary>
		/// Sets flag to prevent submit event.
		/// </summary>
		public void PreventSubmit()
			=> IsPrevented = true;
	}
}