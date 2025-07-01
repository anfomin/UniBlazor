using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace UniBlazor;

/// <summary>
/// Base class for components that:
/// <list type="bullet">
/// <item>Creates a service provider scope.</item>
/// <item>Has <see cref="Navigation"/> manager.</item>
/// <item>Has <see cref="Aborted"/> cancellation token that requests cancellation when the component is disposed.</item>
/// <item>Checks for parameter equality via <see cref="ComponentHelper.IsParametersEqual"/> if <see cref="DistinctParameters"/> is <see langword="true"/>.</item>
/// </list>
/// </summary>
public class UniComponentBase : OwningComponentBase, IAsyncDisposable, IDisposable
{
	readonly CancellationTokenSource _abortedSource = new();
	CancellationToken? _abortedToken;
	IReadOnlyDictionary<string, object?>? _parametersPrev;
	bool _parametersInitialized;

	/// <summary>
	/// Gets the <see cref="NavigationManager"/> for navigation and URI manipulation.
	/// </summary>
	[Inject]
	protected NavigationManager Navigation { get; private set; } = default!;

	/// <summary>
	/// Gets a cancellation token that is triggered when the component is disposed.
	/// </summary>
	protected CancellationToken Aborted => _abortedToken ??= _abortedSource.Token;

	/// <summary>
	/// Gets a value determining if the component has been rendered.
	/// </summary>
	protected bool IsRendered { get; private set; }

	/// <summary>
	/// Gets a value determining if the component has been rendered after first initialization and parameters set.
	/// </summary>
	protected bool IsRenderedAfterParams { get; private set; }

	/// <summary>
	/// Returns if only distinct parameters should trigger OnParametersSet.
	/// Default <see langword="true"/>.
	/// </summary>
	protected virtual bool DistinctParameters => true;

	/// <inheritdoc />
	async ValueTask IAsyncDisposable.DisposeAsync()
	{
		if (!IsDisposed)
		{
			await DisposeAsyncCore().ConfigureAwait(false);
			((IDisposable)this).Dispose();
		}
	}

	/// <summary>
	/// This method is called when the component is disposed asynchronously.
	/// </summary>
	protected virtual ValueTask DisposeAsyncCore()
		=> ValueTask.CompletedTask;

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (disposing)
		{
			_abortedSource.Cancel();
			_abortedSource.Dispose();
		}
	}

	/// <summary>
	/// Calls <see cref="ComponentBase.StateHasChanged"/> to notify the component that its state has changed.
	/// When applicable, this will cause the component to be re-rendered.
	/// </summary>
	public void RefreshState()
	{
		StateHasChanged();
	}

	/// <inheritdoc />
	public override async Task SetParametersAsync(ParameterView parameters)
	{
		if (DistinctParameters)
		{
			var parametersNew = parameters.ToDictionary();
			var parametersPrev = _parametersPrev;
			_parametersPrev = parametersNew;
			if (ComponentHelper.IsParametersEqual(parametersNew, parametersPrev))
				return;
		}
		await base.SetParametersAsync(parameters);
		_parametersInitialized = true;
	}

	/// <inheritdoc />
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await base.OnAfterRenderAsync(firstRender);
		if (firstRender)
		{
			try
			{
				await InitializeJSAsync();
			}
			catch (JSDisconnectedException) { }
			IsRendered = true;
		}
		if (_parametersInitialized)
			IsRenderedAfterParams = true;
	}

	/// <summary>
	/// This method is called after first component render inside try-catch block
	/// to skip <see cref="JSDisconnectedException"/>.
	/// Override this method to call JavaScript runtime.
	/// </summary>
	protected virtual Task InitializeJSAsync()
		=> Task.CompletedTask;
}