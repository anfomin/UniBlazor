using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UniBlazor;
using UniBlazor.Internal;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// UniBlazor extensions for services registration.
/// </summary>
public static class UniBlazorServiceExtensions
{
	/// <summary>
	/// Registers <see cref="UniTimeProvider"/> that get timezone from cookie or browser via JS interop.
	/// </summary>
	public static IServiceCollection AddUniBrowserTime(this IServiceCollection services)
	{
		services.AddHttpContextAccessor();
		services.AddScoped<TimeProvider, UniTimeProvider>();
		services.AddScoped<CircuitHandler, BrowserTimeCircuitHandler>();
		return services;
	}

	/// <summary>
	/// Adds <see cref="CircuitServicesAccessor"/> that provides access to Blazor circuit services.
	/// </summary>
	public static IServiceCollection AddCircuitServicesAccessor(this IServiceCollection services)
	{
		services.AddScoped<CircuitServicesAccessor>();
		services.AddScoped<CircuitHandler, CircuitServicesAccessorHandler>();
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