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

	/// <summary>
	/// Register cascading value supplier for complex object properties marked with <see cref="SupplyComplexFromQueryAttribute"/>.
	/// </summary>
	public static IServiceCollection AddCascadingSupplyComplexFromQuery(this IServiceCollection services)
	{
		services.AddScoped(SupplyComplexFromQueryProviderProxy.ICascadingValueSupplierType, SupplyComplexFromQueryProviderProxy.CreateProxy);
		services.TryAddScoped<IComplexObjectBinder, DefaultComplexBinder>();
		return services;
	}
}