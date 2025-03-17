namespace UniBlazor;

/// <summary>
/// Used for creating and binding complex objects for <see cref="SupplyComplexFromQueryProvider"/>.
/// </summary>
public interface IComplexObjectBinder
{
	/// <summary>
	/// Creates an object of the specified type and sets its properties from the query string.
	/// </summary>
	/// <param name="type">Type of object to create.</param>
	/// <param name="queryString">Query string values.</param>
	object Create(Type type, Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>>? queryString);

	/// <summary>
	/// Bind object value from the query string.
	/// </summary>
	/// <param name="obj">Object to bind to.</param>
	/// <param name="queryString">Query string values.</param>
	void Bind(object obj, Dictionary<ReadOnlyMemory<char>, ReadOnlyMemory<char>> queryString);
}