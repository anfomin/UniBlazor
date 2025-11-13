using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UniBlazor;
using UniBlazor.Internal;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// UniBlazor extensions for services registration.
/// </summary>
public static class UniBlazorExtensions
{
	extension(IServiceCollection services)
	{
		/// <summary>
		/// Registers browser <see cref="ILocalStorage"/> and <see cref="ISessionStorage"/>.
		/// </summary>
		public IServiceCollection AddUniBrowserStorage()
		{
			services.AddScoped<ILocalStorage, BrowserLocalStorage>();
			services.AddScoped<ISessionStorage, BrowserSessionStorage>();
			return services;
		}

		/// <summary>
		/// Registers <see cref="IUserTimeProvider"/> as <see cref="UniTimeProvider"/> that get timezone from cookie or browser via JS interop.
		/// </summary>
		public IServiceCollection AddUniBrowserTime()
		{
			services.AddHttpContextAccessor();
			services.AddScoped<IUserTimeProvider, UniTimeProvider>();
			services.AddScoped<CircuitHandler, BrowserTimeCircuitHandler>();
			return services;
		}

		/// <summary>
		/// Adds <see cref="CircuitServicesAccessor"/> that provides access to Blazor circuit services.
		/// </summary>
		public IServiceCollection AddCircuitServicesAccessor()
		{
			services.AddScoped<CircuitServicesAccessor>();
			services.AddScoped<CircuitHandler, CircuitServicesAccessorHandler>();
			return services;
		}

		/// <summary>
		/// Register cascading value supplier for complex object properties marked with <see cref="SupplyComplexFromQueryAttribute"/>.
		/// </summary>
		public IServiceCollection AddCascadingSupplyComplexFromQuery()
		{
			services.AddScoped(SupplyComplexFromQueryProviderProxy.ICascadingValueSupplierType, SupplyComplexFromQueryProviderProxy.CreateProxy);
			services.TryAddScoped<IComplexObjectBinder, DefaultComplexBinder>();
			return services;
		}
	}
}