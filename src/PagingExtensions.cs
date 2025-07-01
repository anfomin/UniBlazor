using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace UniBlazor;

/// <summary>
/// Provides extension methods for paging queries based on <see cref="ItemsProviderRequest"/> and <see cref="DataRequest{T}"/>.
/// </summary>
public static class PagingExtensions
{
	/// <summary>
	/// Applies skip and take according to the <paramref name="request"/>.
	/// </summary>
	public static IQueryable<T> Page<T>(this IQueryable<T> query, ItemsProviderRequest request)
	{
		if (request.StartIndex > 0)
			query = query.Skip(request.StartIndex);
		return query.Take(request.Count);
	}

	/// <summary>
	/// Applies skip and take according to the <paramref name="request"/>. Each request item is multiplied by <paramref name="chunkSize"/>.
	/// </summary>
	/// <param name="chunkSize">Chunk size for request item.</param>
	public static IQueryable<T> PageChunked<T>(this IQueryable<T> query, ItemsProviderRequest request, int chunkSize)
	{
		if (request.StartIndex > 0)
			query = query.Skip(request.StartIndex * chunkSize);
		return query.Take(request.Count * chunkSize);
	}

	/// <summary>
	/// Applies skip and take according to the <paramref name="request"/>.
	/// </summary>
	public static IEnumerable<T> Page<T>(this IEnumerable<T> query, ItemsProviderRequest request)
	{
		if (request.StartIndex > 0)
			query = query.Skip(request.StartIndex);
		return query.Take(request.Count);
	}

	/// <summary>
	/// Applies skip and take according to the <paramref name="request"/>.
	/// </summary>
	public static IQueryable<T> Page<T>(this IQueryable<T> query, DataRequest<T> request)
	{
		if (request.StartIndex > 0)
			query = query.Skip(request.StartIndex);
		return query.Take(request.Count);
	}

	/// <summary>
	/// Applies skip and take according to the <paramref name="request"/>.
	/// </summary>
	public static IEnumerable<T> Page<T>(this IEnumerable<T> query, DataRequest<T> request)
	{
		if (request.StartIndex > 0)
			query = query.Skip(request.StartIndex);
		return query.Take(request.Count);
	}
}