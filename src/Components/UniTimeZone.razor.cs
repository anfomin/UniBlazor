using Microsoft.AspNetCore.Components;

namespace UniBlazor;

/// <summary>
/// Displays user's local time zone information from <see cref="TimeProvider"/>.
/// </summary>
public sealed partial class UniTimeZone : ComponentBase, IDisposable
{
	/// <summary>
	/// Gets user time provider.
	/// </summary>
	[Inject]
	IUserTimeProvider TimeProvider { get; set; } = null!;

	protected override void OnInitialized()
	{
		TimeProvider.TimeZoneChanged += TimeZoneChanged;
	}

	public void Dispose()
	{
		TimeProvider.TimeZoneChanged -= TimeZoneChanged;
	}

	void TimeZoneChanged(object? sender, TimeZoneInfo e)
	{
		StateHasChanged();
	}
}