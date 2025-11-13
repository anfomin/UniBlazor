using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.JSInterop;

namespace UniBlazor.Internal;

/// <summary>
/// Circuit handler that sets browser timezone to <see cref="UniTimeProvider"/> when circuit is opened.
/// </summary>
public class BrowserTimeCircuitHandler(
	ILogger<BrowserTimeCircuitHandler> logger,
	ITimeProvider timeProvider,
	IJSRuntime js
) : CircuitHandler
{
	readonly ILogger _logger = logger;
	readonly ITimeProvider _timeProvider = timeProvider;
	readonly IJSRuntime _js = js;

	public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
	{
		if (_timeProvider is not UniTimeProvider timeProvider)
		{
			_logger.LogWarning("{Type} must be registered to get browser timezone", nameof(UniTimeProvider));
			return;
		}

		try
		{
			await using var internalModule = await _js.ImportInternalModuleAsync(cancellationToken);
			string timeZone = await internalModule.InvokeAsync<string>("initTimeZone", cancellationToken);
			timeProvider.SetBrowserTimeZone(timeZone);
		}
		catch (JSDisconnectedException) { }
	}
}