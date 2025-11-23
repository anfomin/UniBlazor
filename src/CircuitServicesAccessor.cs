
namespace UniBlazor;

/// <summary>
/// Provides access to Blazor circuit services.
/// </summary>
public class CircuitServicesAccessor : IServiceProvider
{
	static readonly AsyncLocal<IServiceProvider?> CircuitServices = new();

	/// <summary>
	/// Called when a circuit starts.
	/// </summary>
	/// <param name="circuitServices">Circuit services provider.</param>
	public void StartCircuit(IServiceProvider circuitServices)
		=> CircuitServices.Value = circuitServices;

	/// <summary>
	/// Called when a circuit ends.
	/// </summary>
	public void EndCircuit()
		=> CircuitServices.Value = null;

	/// <summary>
	/// Gets a service from the current circuit's service provider.
	/// </summary>
	/// <param name="serviceType">Service type to get.</param>
	/// <returns>Service instance.</returns>
	/// <exception cref="InvalidOperationException">Method invoked outside of Blazor circuit.</exception>
	public object? GetService(Type serviceType)
	{
		if (CircuitServices.Value is null)
			throw new InvalidOperationException("Circuit services not available");
		return CircuitServices.Value.GetService(serviceType);
	}
}