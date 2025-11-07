using Microsoft.AspNetCore.Components;

namespace UniBlazor;

/// <summary>
/// Displays <see cref="DateTime"/> or <see cref="DateTimeOffset"/> in the specified
/// <see cref="TimeZone"/> or user's local time zone from <see cref="TimeProvider"/>.
/// </summary>
public sealed partial class UniTime : ComponentBase, IDisposable
{
	/// <summary>
	/// Gets time provider.
	/// </summary>
	[Inject]
	TimeProvider TimeProvider { get; set; } = null!;

	/// <summary>
	/// Gets or sets <see cref="DateTime"/> to display.
	/// </summary>
	[Parameter]
	public DateTime? Value { get; set; }

	/// <summary>
	/// Gets or sets <see cref="DateTimeOffset"/> to display.
	/// </summary>
	[Parameter]
	public DateTimeOffset? Offset { get; set; }

	/// <summary>
	/// Gets or sets time zone to display <see cref="Value"/> or <see cref="Offset"/> in.
	/// If null then uses user's local time zone from <see cref="TimeProvider"/>.
	/// </summary>
	[Parameter]
	public TimeZoneInfo? TimeZone { get; set; }

	/// <summary>
	/// Gets or sets datetime format string.
	/// </summary>
	[Parameter]
	public string? Format { get; set; }

	/// <summary>
	/// Gets or sets placeholder text to display when <see cref="Value"/> and <see cref="Offset"/> are null.
	/// If <c>null</c> then displays "—" (em dash).
	/// If empty then displays nothing.
	/// </summary>
	[Parameter]
	public string? Placeholder { get; set; }

	protected override void OnInitialized()
	{
		if (TimeProvider is UniTimeProvider browserTimeProvider)
			browserTimeProvider.LocalTimeZoneChanged += LocalTimeZoneChanged;
	}

	public void Dispose()
	{
		if (TimeProvider is UniTimeProvider browserTimeProvider)
			browserTimeProvider.LocalTimeZoneChanged -= LocalTimeZoneChanged;
	}

	void LocalTimeZoneChanged(object? sender, TimeZoneInfo e)
	{
		if (TimeZone == null)
			StateHasChanged();
	}
}