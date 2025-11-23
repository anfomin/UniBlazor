using Microsoft.AspNetCore.Http;

namespace UniBlazor;

/// <summary>
/// Provides the current time in the browser's local time zone.
/// Timezone is determined from cookie or set from browser via JS interop.
/// </summary>
public sealed class UniTimeProvider : TimeProvider, ITimeProvider
{
	const string CookieName = "uni-timezone";
	readonly ILogger _logger;
	readonly TimeZoneInfo? _cookieTimeZone;
	TimeZoneInfo? _browserTimeZone;

	/// <summary>
	/// Gets a value indicating whether the browser's time zone has been set.
	/// </summary>
	public bool IsTimeZoneSet => _browserTimeZone is not null;

	public event EventHandler<TimeZoneInfo>? TimeZoneChanged;

	public override TimeZoneInfo LocalTimeZone
		=> _browserTimeZone ?? _cookieTimeZone ?? base.LocalTimeZone;

	/// <summary>
	/// Initializes a new instance of the <see cref="UniTimeProvider"/> class.
	/// </summary>
	public UniTimeProvider(ILogger<UniTimeProvider> logger, IHttpContextAccessor httpContextAccessor)
	{
		_logger = logger;
		if (httpContextAccessor.HttpContext is { } context
			&& context.Request.Cookies.TryGetValue(CookieName, out var timeZoneId)
			&& !string.IsNullOrEmpty(timeZoneId)
			&& TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out var timeZone))
		{
			logger.LogDebug("Timezone set from cookie to {TimeZone}", timeZoneId);
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
			_logger.LogDebug("Timezone set from browser to {TimeZone}", timeZoneId);
			_browserTimeZone = timeZone;
			TimeZoneChanged?.Invoke(this, LocalTimeZone);
		}
	}
}