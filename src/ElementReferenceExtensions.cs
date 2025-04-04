using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Provides extensions for <see cref="ElementReference"/>.
/// </summary>
public static class ElementReferenceExtensions
{
	/// <summary>
	/// Scrolls to the element.
	/// </summary>
	/// <param name="elementReference">Element to scroll to.</param>
	/// <param name="behavior">Scroll behaviour. Default smooth.</param>
	/// <param name="position">Scroll block position. Default start.</param>
	public static ValueTask ScrollIntoViewAsync(this ElementReference elementReference, ScrollBehavior behavior = ScrollBehavior.Smooth, ScrollPosition position = ScrollPosition.Start)
	{
		var js = elementReference.GetJsRuntime();
		return js.InvokeVoidAsync("UniBlazor.scrollIntoView", elementReference, behavior.ToString().ToLower(), position.ToString().ToLower());
	}

	internal static IJSRuntime GetJsRuntime(this ElementReference elementReference)
	{
		if (elementReference.Context is not WebElementReferenceContext context)
			throw new InvalidOperationException("ElementReference has not been configured correctly.");
		return GetJsRuntime(context);
	}

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<JSRuntime>k__BackingField")]
	static extern ref IJSRuntime GetJsRuntime(WebElementReferenceContext context);
}

public enum ScrollPosition
{
	Start,
	Center,
	End,
	Nearest
}

public enum ScrollBehavior
{
	Auto,
	Instant,
	Smooth
}