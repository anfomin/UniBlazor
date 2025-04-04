using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace UniBlazor;

/// <summary>
/// Provides parameter marked by <see cref="SupplyComplexFromQueryAttribute"/> with values from the query string.
/// </summary>
public class SupplyComplexFromQueryProvider(IComplexObjectBinder binder, NavigationManager navigation) : IDisposable
{
	static readonly object UnboundLifetime;
	static readonly MethodInfo ComponentNotifyCascadingValueChanged;

	static SupplyComplexFromQueryProvider()
	{
		var typeParameterViewLifetime = typeof(IComponent).Assembly.GetType("Microsoft.AspNetCore.Components.Rendering.ParameterViewLifetime")!;
		var unboundField = typeParameterViewLifetime.GetField("Unbound", BindingFlags.Static | BindingFlags.Public)!;
		UnboundLifetime = unboundField.GetValue(null)!;
		ComponentNotifyCascadingValueChanged = typeof(ComponentState).GetMethod("NotifyCascadingValueChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;
	}

	readonly IComplexObjectBinder _binder = binder;
	readonly NavigationManager _navigation = navigation;
	readonly HashSet<(ComponentState ComponentState, Type PropertyType)> _subscribers = [];
	readonly Dictionary<Type, int> _subscribedParameters = [];
	readonly Dictionary<Type, object> _values = [];
	bool _isSubscribedToLocationChanges;
	string? _lastUri;
	readonly Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>> _queryValues = new(ReadOnlyMemoryCharComparer.OrdinalIgnoreCase);

	public bool IsFixed { get; } = false;

	public void Dispose()
		=> UnsubscribeFromLocationChanges();

	public bool CanSupplyValue(in CascadingParameterInfo parameterInfo)
		=> parameterInfo.Attribute is SupplyComplexFromQueryAttribute;

	public object? GetCurrentValue(in CascadingParameterInfo parameterInfo)
	{
		if (TryUpdateUri(_navigation.Uri))
			UpdateValues();
		return _values.GetOrCreate(parameterInfo.PropertyType, type => _binder.Create(type, _queryValues));
	}

	public void Subscribe(ComponentState subscriber, in CascadingParameterInfo parameterInfo)
	{
		var propType = parameterInfo.PropertyType;
		_subscribers.Add((subscriber, propType));
		_subscribedParameters[propType] = _subscribedParameters.TryGetValue(propType, out int value) ? value + 1 : 1;
		SubscribeToLocationChanges();
	}

	public void Unsubscribe(ComponentState subscriber, in CascadingParameterInfo parameterInfo)
	{
		var propType = parameterInfo.PropertyType;
		_subscribers.Remove((subscriber, propType));
		_subscribedParameters[propType]--;
		if (_subscribedParameters[propType] == 0)
		{
			_subscribedParameters.Remove(propType);
			_values.Remove(propType);
		}
		if (_subscribers.Count == 0)
			UnsubscribeFromLocationChanges();
	}

	/// <summary>
	/// Updates the last URI and query values if the URI has changed.
	/// </summary>
	bool TryUpdateUri(string uri)
	{
		if (uri == _lastUri)
			return false;

		var queryString = QueryStringHelper.GetQueryString(uri);
		if (_lastUri != null && queryString.Span.SequenceEqual(QueryStringHelper.GetQueryString(_lastUri).Span))
		{
			_lastUri = uri;
			return false;
		}

		_lastUri = uri;
		_queryValues.Clear();
		QueryStringEnumerable queryStringEnumerable = new(QueryStringHelper.GetQueryString(uri));
		foreach (var pair in queryStringEnumerable)
		{
			var decodedName = pair.DecodeName();
			var decodedValue = pair.DecodeValue();
			_queryValues[decodedName] = decodedValue;
		}
		return true;
	}

	/// <summary>
	/// Updates values of subscribed parameters and returns types of changed parameters.
	/// </summary>
	HashSet<Type> UpdateValues()
	{
		HashSet<Type> changedTypes = [];
		foreach (var type in _subscribedParameters.Keys)
		{
			var value = _binder.Create(type, _queryValues);
			if (!_values.TryGetValue(type, out var current) || current != value)
			{
				_values[type] = value;
				changedTypes.Add(type);
			}
		}
		return changedTypes;
	}

	void SubscribeToLocationChanges()
	{
		if (_isSubscribedToLocationChanges)
			return;
		_isSubscribedToLocationChanges = true;
		_navigation.LocationChanged += OnLocationChanged;
	}

	void UnsubscribeFromLocationChanges()
	{
		if (!_isSubscribedToLocationChanges)
			return;
		_isSubscribedToLocationChanges = false;
		_navigation.LocationChanged -= OnLocationChanged;
	}

	void OnLocationChanged(object? sender, LocationChangedEventArgs args)
	{
		if (!TryUpdateUri(args.Location))
			return;

		// update changed values and notify subscribers
		var changedTypes = UpdateValues();
		foreach (var group in _subscribers.GroupBy(s => s.ComponentState))
		{
			if (group.Any(s => changedTypes.Contains(s.PropertyType)))
				ComponentNotifyCascadingValueChanged.Invoke(group.Key, [UnboundLifetime]);
		}
	}
}

/// <summary>
/// Proxy for <see cref="SupplyComplexFromQueryProvider"/> that implements <see cref="ICascadingValueSupplier"/>.
/// Required because ICascadingValueSupplier is internal.
/// </summary>
public class SupplyComplexFromQueryProviderProxy : DispatchProxy
{
	SupplyComplexFromQueryProvider _implementation = default!;

	protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
		=> _implementation.GetType().GetMethod(targetMethod!.Name, BindingFlags.Instance | BindingFlags.Public)!.Invoke(_implementation, args);

	public static Type ICascadingValueSupplierType { get; }
		= typeof(IComponent).Assembly.GetType("Microsoft.AspNetCore.Components.ICascadingValueSupplier")!;

	public static object CreateProxy(IServiceProvider services)
		=> CreateProxy(services.GetRequiredService<IComplexObjectBinder>(), services.GetRequiredService<NavigationManager>());

	public static object CreateProxy(IComplexObjectBinder binder, NavigationManager navigation)
	{
		var proxy = (SupplyComplexFromQueryProviderProxy)Create(ICascadingValueSupplierType, typeof(SupplyComplexFromQueryProviderProxy));
		proxy._implementation = new(binder, navigation);
		return proxy;
	}
}