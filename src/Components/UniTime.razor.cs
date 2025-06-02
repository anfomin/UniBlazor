using Microsoft.AspNetCore.Components;

namespace UniBlazor;

/// <summary>
/// Displays datetime in user's local time zone.
/// </summary>
public sealed partial class UniTime : ComponentBase, IDisposable
{
	/// <summary>
	/// Gets time provider.
	/// </summary>
	[Inject]
	TimeProvider TimeProvider { get; set; } = null!;

	/// <summary>
	/// Gets or sets datetime to display.
	/// </summary>
	[Parameter]
	public DateTime? Value { get; set; }

	/// <summary>
	/// Gets or sets datetime format string.
	/// </summary>
	[Parameter]
	public string? Format { get; set; }

	/// <summary>
	/// Gets or sets placeholder text to display when <see cref="Value"/> is null.
	/// If <c>null</c> then displays "—" (em dash).
	/// If empty then displays nothing.
	/// </summary>
	[Parameter]
	public string? Placeholder { get; set; }

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		if (TimeProvider is BrowserTimeProvider browserTimeProvider)
			browserTimeProvider.LocalTimeZoneChanged += LocalTimeZoneChanged;
	}

	/// <inheritdoc />
	public void Dispose()
	{
		if (TimeProvider is BrowserTimeProvider browserTimeProvider)
			browserTimeProvider.LocalTimeZoneChanged -= LocalTimeZoneChanged;
	}

	void LocalTimeZoneChanged(object? sender, TimeZoneInfo e)
		=> StateHasChanged();
}