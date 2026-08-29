using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace UniBlazor.Internal;

/// <summary>
/// Provides access to the internal <see cref="ICascadingValueSupplier"/>.
/// </summary>
public static class CascadingValueSupplierProxy
{
	/// <summary>
	/// Gets the <see cref="Type"/> of the internal <c>ICascadingValueSupplier</c> interface from the Microsoft.AspNetCore.Components assembly.
	/// </summary>
	public static Type CascadingValueSupplierInterface { get; }
		= typeof(IComponent).Assembly.GetType("Microsoft.AspNetCore.Components.ICascadingValueSupplier")!;
}

/// <summary>
/// Proxy for <typeparamref name="T"/> that implements internal <see cref="ICascadingValueSupplier"/> via <see cref="IUniCascadingValueSupplier"/>.
/// </summary>
public class CascadingValueSupplierProxy<T> : DispatchProxy
	where T : class, IUniCascadingValueSupplier
{
	static readonly ObjectFactory<T> Factory = ActivatorUtilities.CreateFactory<T>([]);
	T _implementation = default!;

	protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
		=> _implementation.GetType().GetMethod(targetMethod!.Name, BindingFlags.Instance | BindingFlags.Public)!.Invoke(_implementation, args);

	/// <summary>
	/// Creates a proxy instance of <typeparamref name="T"/> that implements the internal <see cref="ICascadingValueSupplier"/> interface.
	/// </summary>
	/// <param name="provider">The <see cref="IServiceProvider"/> used to resolve dependencies.</param>
	/// <returns>A proxy instance of <typeparamref name="T"/>.</returns>
	public static object CreateProxy(IServiceProvider provider)
	{
		var proxy = (CascadingValueSupplierProxy<T>)Create(CascadingValueSupplierProxy.CascadingValueSupplierInterface, typeof(CascadingValueSupplierProxy<T>));
		proxy._implementation = Factory(provider, null);
		return proxy;
	}
}