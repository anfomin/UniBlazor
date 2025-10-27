using Microsoft.AspNetCore.Http;

namespace UniBlazor;

/// <summary>
/// Provides the current time in the browser's local time zone.
/// </summary>
public sealed class UniTimeProvider : TimeProvider
{
	const string CookieName = "uni-timezone";
	readonly TimeZoneInfo? _cookieTimeZone;
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
		=> _browserTimeZone ?? _cookieTimeZone ?? base.LocalTimeZone;

	public UniTimeProvider(IHttpContextAccessor httpContextAccessor)
	{
		if (httpContextAccessor.HttpContext is { } context
			&& context.Request.Cookies.TryGetValue(CookieName, out var timeZoneId)
			&& !string.IsNullOrEmpty(timeZoneId)
			&& TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var timeZone))
		{
			_cookieTimeZone = timeZone;
		}
	}

	/// <summary>
	/// Sets browser timezone identifier.
	/// </summary>
	public void SetBrowserTimeZone(string timeZoneId)
	{
		if (!TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var timeZone))
			timeZone = null;
		if (!LocalTimeZone.Equals(timeZone))
		{
			_browserTimeZone = timeZone;
			LocalTimeZoneChanged?.Invoke(this, LocalTimeZone);
		}
	}
}