using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
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
public class UniComponentBase : ComponentBase, IDisposable, IAsyncDisposable
{
	readonly CancellationTokenSource _abortedSource;
	AsyncServiceScope? _scope;
	IReadOnlyDictionary<string, object?>? _parametersPrev;
	bool _parametersInitialized;

	/// <summary>
	/// Gets <see cref="IServiceScopeFactory"/> to create service provider scopes.
	/// </summary>
	[Inject]
	protected IServiceScopeFactory ScopeFactory { get; private set; } = default!;

	/// <summary>
	/// Gets the scoped <see cref="IServiceProvider"/> that is associated with this component.
	/// </summary>
	protected IServiceProvider ScopedServices
	{
		get
		{
			if (ScopeFactory is null)
				throw new InvalidOperationException("Services cannot be accessed before the component is initialized.");

			ObjectDisposedException.ThrowIf(IsDisposed, this);
			_scope ??= ScopeFactory.CreateAsyncScope();
			return _scope.Value.ServiceProvider;
		}
	}

	/// <summary>
	/// Gets the <see cref="NavigationManager"/> for navigation and URI manipulation.
	/// </summary>
	[Inject]
	protected NavigationManager Navigation { get; private set; } = default!;

	/// <summary>
	/// Gets a cancellation token that is triggered when the component is disposed.
	/// Token is canceled before <see cref="ScopedServices"/> disposing.
	/// </summary>
	protected CancellationToken Aborted { get; }

	/// <summary>
	/// Gets a value determining if the component and associated services have been disposed.
	/// </summary>
	protected bool IsDisposed { get; private set; }

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

	/// <summary>
	/// Initializes a new instance of the <see cref="UniComponentBase"/> class.
	/// </summary>
	public UniComponentBase()
	{
		_abortedSource = new();
		Aborted = _abortedSource.Token;
	}

	void IDisposable.Dispose()
	{
		if (!IsDisposed)
		{
			_abortedSource.Cancel();
			_abortedSource.Dispose();
			Dispose(disposing: true);
			_scope?.Dispose();
			_scope = null;
			IsDisposed = true;
		}
	}

	/// <summary>
	/// This method is called when the component is disposed.
	/// Invoked before the <see cref="Aborted"/> cancelled and <see cref="ScopedServices"/> are disposed.
	/// </summary>
	/// <param name="disposing">
	/// <see langword="true"/> if called from <see cref="IDisposable.Dispose"/>;
	/// </param>
	protected virtual void Dispose(bool disposing) { }

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

	/// <summary>
	/// Calls <see cref="ComponentBase.StateHasChanged"/> to notify the component that its state has changed.
	/// When applicable, this will cause the component to be re-rendered.
	/// </summary>
	public void RefreshState()
	{
		StateHasChanged();
	}

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
			catch (JSException ex) when (ex.Message == "null") { }
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