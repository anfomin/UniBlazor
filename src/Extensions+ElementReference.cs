using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace UniBlazor;

public static partial class Extensions
{
	extension(ElementReference elementReference)
	{
		/// <summary>
		/// Scrolls to the <see cref="ElementReference"/>.
		/// </summary>
		/// <param name="behavior">Scroll behavior. Default smooth.</param>
		/// <param name="position">Scroll block position. Default start.</param>
		public async ValueTask ScrollIntoViewAsync(ScrollBehavior behavior = ScrollBehavior.Smooth, ScrollPosition position = ScrollPosition.Start, CancellationToken cancellationToken = default)
		{
			var js = elementReference.GetJsRuntime();
			await using var internalModule = await js.ImportInternalModuleAsync(cancellationToken);
			await internalModule.InvokeVoidAsync("scrollIntoView", cancellationToken, elementReference, behavior.ToString().ToLower(), position.ToString().ToLower());
		}

		internal IJSRuntime GetJsRuntime()
		{
			if (elementReference.Context is not WebElementReferenceContext context)
				throw new InvalidOperationException("ElementReference has not been configured correctly.");
			return GetJsRuntime(context);
		}
	}

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<JSRuntime>k__BackingField")]
	static extern ref IJSRuntime GetJsRuntime(WebElementReferenceContext context);
}

/// <summary>
/// Represents vertical scroll positions.
/// </summary>
public enum ScrollPosition
{
	Start,
	Center,
	End,
	Nearest
}

/// <summary>
/// Represents scroll behavior.
/// </summary>
public enum ScrollBehavior
{
	Auto,
	Instant,
	Smooth
}