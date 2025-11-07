using Microsoft.AspNetCore.Components;

namespace UniBlazor;

/// <summary>
/// Displays user's local time zone information from <see cref="TimeProvider"/>.
/// </summary>
public sealed partial class UniTimeZone : ComponentBase, IDisposable
{
	/// <summary>
	/// Gets time provider.
	/// </summary>
	[Inject]
	TimeProvider TimeProvider { get; set; } = null!;

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
		StateHasChanged();
	}
}