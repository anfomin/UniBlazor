using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Invokes JS script to get browser time zone and sets it to <see cref="UniTimeProvider"/>.
/// </summary>
public sealed partial class UniBrowserTimeZone : ComponentBase
{
	/// <summary>
	/// Gets logger.
	/// </summary>
	[Inject]
	ILogger<UniBrowserTimeZone> Logger { get; set; } = null!;

	/// <summary>
	/// Gets time provider.
	/// </summary>
	[Inject]
	TimeProvider TimeProvider { get; set; } = null!;

	/// <summary>
	/// Gets JS runtime.
	/// </summary>
	[Inject]
	IJSRuntime JS { get; set; } = null!;

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender && TimeProvider is UniTimeProvider { IsTimeZoneSet: false } browserTimeProvider)
		{
			try
			{
				string timeZone = await JS.InvokeAsync<string>("UniBlazor.getTimeZone");
				browserTimeProvider.SetBrowserTimeZone(timeZone);
				Logger.LogDebug("Browser timezone set to {TimeZone}", timeZone);
			}
			catch (JSDisconnectedException) { }
		}
	}
}