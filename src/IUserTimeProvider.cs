namespace UniBlazor;

/// <summary>
/// Provides an abstraction for user time.
/// </summary>
public interface IUserTimeProvider
{
	/// <summary>
	/// Gets a <see cref="TimeZoneInfo" /> object that represents user time zone.
	/// </summary>
	TimeZoneInfo LocalTimeZone { get; }

	/// <summary>
	/// Fires event when the <see cref="LocalTimeZone"/> has changed.
	/// </summary>
	event EventHandler<TimeZoneInfo>? TimeZoneChanged;

	/// <summary>
	/// Gets a <see cref="DateTimeOffset" /> value whose date and time are set to the current
	/// Coordinated Universal Time (UTC) date and time and whose offset is Zero.
	/// </summary>
	DateTimeOffset GetUtcNow();

	/// <summary>
	/// Gets a <see cref="DateTimeOffset" /> value that is set to the user date and time according
	/// to this <see cref="IUserTimeProvider" />'s notion of time based on <see cref="GetUtcNow" />,
	/// with the offset set to the <see cref="LocalTimeZone" />'s offset from Coordinated Universal Time (UTC).
	/// </summary>
	DateTimeOffset GetLocalNow();
}