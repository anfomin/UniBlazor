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
			services.TryAddScoped<ILocalStorage, BrowserLocalStorage>();
			services.TryAddScoped<ISessionStorage, BrowserSessionStorage>();
			return services;
		}

		/// <summary>
		/// Registers browser <see cref="IClipboard"/>.
		/// </summary>
		public IServiceCollection AddUniBrowserClipboard()
		{
			services.TryAddScoped<IClipboard, BrowserClipboard>();
			return services;
		}

		/// <summary>
		/// Registers <see cref="ITimeProvider"/> as <see cref="BrowserTimeProvider"/> that get timezone from cookie or browser via JS interop.
		/// </summary>
		public IServiceCollection AddUniBrowserTime()
		{
			services.AddHttpContextAccessor();
			services.TryAddScoped<ITimeProvider, BrowserTimeProvider>();
			services.TryAddScoped<CircuitHandler, BrowserTimeCircuitHandler>();
			return services;
		}

		/// <summary>
		/// Adds <see cref="CircuitServicesAccessor"/> that provides access to Blazor circuit services.
		/// </summary>
		public IServiceCollection AddCircuitServicesAccessor()
		{
			services.TryAddScoped<CircuitServicesAccessor>();
			services.TryAddScoped<CircuitHandler, CircuitServicesAccessorHandler>();
			return services;
		}

		/// <summary>
		/// Registers <typeparamref name="T"/> cascading value supplier.
		/// </summary>
		public IServiceCollection AddCascadingValueSupplier<T>()
			where T : class, IUniCascadingValueSupplier
			=> services.AddScoped(CascadingValueSupplierProxy.CascadingValueSupplierInterface, CascadingValueSupplierProxy<T>.CreateProxy);

		/// <summary>
		/// Register cascading value supplier for complex object properties marked with <see cref="SupplyComplexFromQueryAttribute"/>.
		/// </summary>
		public IServiceCollection AddCascadingSupplyComplexFromQuery()
		{
			services.AddCascadingValueSupplier<SupplyComplexFromQueryProvider>();
			services.TryAddScoped<IComplexObjectBinder, DefaultComplexBinder>();
			return services;
		}
	}
}