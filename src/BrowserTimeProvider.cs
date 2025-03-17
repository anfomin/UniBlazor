namespace UniBlazor;

/// <summary>
/// Provides the current time in the browser's local time zone.
/// </summary>
public sealed class BrowserTimeProvider : TimeProvider
{
	TimeZoneInfo? _browserTimeZone;

	/// <summary>
	/// Gets a value indicating whether the browser's time zone has been set.
	/// </summary>
	public bool IsTimeZoneSet => _browserTimeZone != null;

	/// <summary>
	/// Fires event when the browser's time zone has changed.
	/// </summary>
	public event EventHandler<TimeZoneInfo>? LocalTimeZoneChanged;

	/// <inheritdoc />
	public override TimeZoneInfo LocalTimeZone
		=> _browserTimeZone ?? base.LocalTimeZone;

	/// <summary>
	/// Sets browser timezone identifier.
	/// </summary>
	public void SetBrowserTimeZone(string timeZoneId)
	{
		if (!TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var timeZoneInfo))
			timeZoneInfo = null;
		if (timeZoneInfo != LocalTimeZone)
		{
			_browserTimeZone = timeZoneInfo;
			LocalTimeZoneChanged?.Invoke(this, LocalTimeZone);
		}
	}
}