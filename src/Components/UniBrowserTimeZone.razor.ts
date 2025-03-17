export function getTimeZone(): string {
	const options = Intl.DateTimeFormat().resolvedOptions();
	return options.timeZone;
}