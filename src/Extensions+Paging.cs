using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace UniBlazor;

public static partial class Extensions
{
	extension<T>(IQueryable<T> query)
	{
		/// <summary>
		/// Applies skip and take according to the <paramref name="request"/>.
		/// </summary>
		public IQueryable<T> Page(ItemsProviderRequest request)
		{
			if (request.StartIndex > 0)
				query = query.Skip(request.StartIndex);
			return query.Take(request.Count);
		}

		/// <summary>
		/// Applies skip and take according to the <paramref name="request"/>. Each request item is multiplied by <paramref name="chunkSize"/>.
		/// </summary>
		/// <param name="chunkSize">Chunk size for request item.</param>
		public IQueryable<T> PageChunked(ItemsProviderRequest request, int chunkSize)
		{
			if (request.StartIndex > 0)
				query = query.Skip(request.StartIndex * chunkSize);
			return query.Take(request.Count * chunkSize);
		}

		/// <summary>
		/// Applies skip and take according to the <paramref name="request"/>.
		/// </summary>
		public IQueryable<T> Page(DataRequest<T> request)
		{
			if (request.StartIndex > 0)
				query = query.Skip(request.StartIndex);
			return query.Take(request.Count);
		}
	}

	extension<T>(IEnumerable<T> query)
	{
		/// <summary>
		/// Applies skip and take according to the <paramref name="request"/>.
		/// </summary>
		public IEnumerable<T> Page(ItemsProviderRequest request)
		{
			if (request.StartIndex > 0)
				query = query.Skip(request.StartIndex);
			return query.Take(request.Count);
		}

		/// <summary>
		/// Applies skip and take according to the <paramref name="request"/>.
		/// </summary>
		public IEnumerable<T> Page(DataRequest<T> request)
		{
			if (request.StartIndex > 0)
				query = query.Skip(request.StartIndex);
			return query.Take(request.Count);
		}
	}
}