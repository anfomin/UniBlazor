namespace UniBlazor;

/// <summary>
/// A function that provides items.
/// </summary>
public delegate ValueTask<DataResult<T>> DataDelegate<T>(DataRequest<T> request);

/// <summary>
/// Represents a request to an <see cref="DataDelegate{T}"/>.
/// </summary>
public readonly record struct DataRequest<T>(
	int StartIndex,
	int Count,
	DataSort? Sort,
	CancellationToken CancellationToken
) {
	/// <inheritdoc />
	public override string ToString()
		=> $"Request from {StartIndex}, count {Count}, sort {Sort}";
}

/// <summary>
/// Represents the result of a <see cref="DataDelegate{T}"/>.
/// </summary>
public readonly record struct DataResult<T>(IEnumerable<T> Items, int TotalItems)
{
	/// <summary>
	/// Initializes a new <see cref="DataResult{T}"/> with no items and a total of 0.
	/// </summary>
	public DataResult() : this([], 0) { }
}