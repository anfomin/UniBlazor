namespace UniBlazor;

public static partial class Extensions
{
	extension(DateOnly)
	{
		/// <summary>
		/// Returns the current date in specified by <see cref="ITimeProvider"/> timezone.
		/// </summary>
		public static DateOnly GetToday(ITimeProvider timeProvider)
			=> DateOnly.GetToday(timeProvider.LocalTimeZone);
	}

	extension(DateTime dateTime)
	{
		/// <summary>
		/// Converts <see cref="DateTime"/> to the timezone specified by <see cref="ITimeProvider"/>.
		/// <see cref="DateTime"/> of kind <see cref="DateTimeKind.Unspecified"/> treated as UTC time.
		/// </summary>
		/// <param name="timeProvider">Time provider that specifies timezone to convert to.</param>
		public DateTime ToTimeZone(ITimeProvider timeProvider)
			=> dateTime.ToTimeZone(timeProvider.LocalTimeZone);
	}

	extension(DateTimeOffset dateTimeOffset)
	{
		/// <summary>
		/// Converts <see cref="DateTimeOffset"/> to the timezone specified by <see cref="ITimeProvider"/>.
		/// </summary>
		/// <param name="timeProvider">Time provider that specifies timezone to convert to.</param>
		public DateTimeOffset ToTimeZone(ITimeProvider timeProvider)
			=> TimeZoneInfo.ConvertTime(dateTimeOffset, timeProvider.LocalTimeZone);
	}
}