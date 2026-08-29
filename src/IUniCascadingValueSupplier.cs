using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace UniBlazor;

/// <summary>
/// Represents a supplier of cascading values for components.
/// This interface matches the internal <see cref="ICascadingValueSupplier"/>.
/// </summary>
public interface IUniCascadingValueSupplier
{
	/// <summary>
	/// Gets a value indicating whether the supplier provides fixed values that do not change over time.
	/// </summary>
	bool IsFixed { get; }

	/// <summary>
	/// Determines whether the supplier can provide a value for the specified cascading parameter.
	/// </summary>
	/// <param name="parameterInfo">The information about the cascading parameter for which to check if a value can be supplied.</param>
	/// <returns><c>true</c> if the supplier can provide a value for the specified cascading parameter; otherwise, <c>false</c>.</returns>
	bool CanSupplyValue(in CascadingParameterInfo parameterInfo);

	/// <summary>
	/// Gets the current value for the specified cascading parameter.
	/// </summary>
	/// <param name="key">The key identifying the cascading parameter.</param>
	/// <param name="parameterInfo">The information about the cascading parameter for which to get the current value.</param>
	/// <returns>The current value of the specified cascading parameter.</returns>
	object? GetCurrentValue(object? key, in CascadingParameterInfo parameterInfo);

	/// <summary>
	/// Subscribes a component to receive updates for the specified cascading parameter.
	/// </summary>
	/// <param name="subscriber">The component state that is subscribing to updates.</param>
	/// <param name="parameterInfo">The information about the cascading parameter for which to receive updates.</param>
	void Subscribe(ComponentState subscriber, in CascadingParameterInfo parameterInfo);

	/// <summary>
	/// Unsubscribes a component from receiving updates for the specified cascading parameter.
	/// </summary>
	/// <param name="subscriber">The component state that is unsubscribing from updates.</param>
	/// <param name="parameterInfo">The information about the cascading parameter for which to stop receiving updates.</param>
	void Unsubscribe(ComponentState subscriber, in CascadingParameterInfo parameterInfo);
}