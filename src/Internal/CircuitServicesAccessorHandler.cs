using Microsoft.AspNetCore.Components.Server.Circuits;

namespace UniBlazor.Internal;

/// <summary>
/// Circuit handler that manages the lifecycle of <see cref="CircuitServicesAccessor"/>.
/// </summary>
public class CircuitServicesAccessorHandler(IServiceProvider services, CircuitServicesAccessor accessor) : CircuitHandler
{
	public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(Func<CircuitInboundActivityContext, Task> next)
		=> async context =>
		{
			accessor.StartCircuit(services);
			await next(context);
			accessor.EndCircuit();
		};
}