using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace UniBlazor;

/// <summary>
/// Provides custom validation for <see cref="EditForm"/> and <see cref="EditContext"/>.
/// </summary>
public class UniValidator : ComponentBase, IDisposable
{
	bool _validated;
	ValidationMessageStore? _messageStore;
	EditContext? _previousEditContext;

	/// <summary>
	/// Specifies custom function that returns validation error messages
	/// or empty if no errors.
	/// </summary>
	[Parameter, EditorRequired]
	public Func<IEnumerable<string>> OnValidate { get; set; } = default!;

	/// <summary>
	/// Gets edit context from cascading parameters.
	/// </summary>
	[CascadingParameter]
	EditContext? EditContext { get; set; }

	protected override void OnInitialized()
	{
		if (EditContext is null)
			throw new InvalidOperationException($"{nameof(UniValidator)} requires a cascading parameter of type {nameof(EditContext)}");

		if (EditContext != _previousEditContext)
		{
			if (_previousEditContext is not null)
			{
				_previousEditContext.OnFieldChanged -= OnFieldChanged;
				_previousEditContext.OnValidationRequested -= OnValidationRequested;
			}

			_validated = false;
			_messageStore = new(EditContext);
			EditContext.OnFieldChanged += OnFieldChanged;
			EditContext.OnValidationRequested += OnValidationRequested;
			_previousEditContext = EditContext;
		}
	}

	public void Dispose()
	{
		if (EditContext is not null)
		{
			EditContext.OnFieldChanged -= OnFieldChanged;
			EditContext.OnValidationRequested -= OnValidationRequested;
		}
	}

	void OnFieldChanged(object? sender, FieldChangedEventArgs e)
	{
		if (_validated)
			Validate();
	}

	void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
	{
		Validate();
		_validated = true;
	}

	/// <summary>
	/// Validates the current edit context and updates the validation message store.
	/// </summary>
	void Validate()
	{
		if (_messageStore is null || EditContext is null)
			return;
		_messageStore.Clear();
		_messageStore.Add(EditContext.Field(string.Empty), OnValidate());
	}
}