using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace UniBlazor;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceExtensions
{
	/// <summary>
	/// Registers browser time provider as the default time provider.
	/// </summary>
	public static IServiceCollection AddUniBrowserTime(this IServiceCollection services)
	{
		services.AddScoped<TimeProvider, BrowserTimeProvider>();
		return services;
	}
}