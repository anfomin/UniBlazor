using System.Drawing;
using Microsoft.AspNetCore.Components.Forms;

namespace UniBlazor;

/// <summary>
/// Represents the data of a file selected from an <see cref="UniFileDnd"/> component.
/// </summary>
public interface IBrowserFileAsync : IBrowserFile, IAsyncDisposable
{
	/// <summary>
	/// Opens the stream for reading the uploaded file asynchronously.
	/// </summary>
	/// <param name="maxAllowedSize">
	/// The maximum number of bytes that can be supplied by the Stream. Defaults to <c>500 KB</c>.
	/// <para>
	/// Calling <see cref="OpenReadStreamAsync(long, CancellationToken)"/> will throw if the
	/// file's size, as specified by <see cref="Size"/> is larger than <paramref name="maxAllowedSize"/>.
	/// By default, if the user supplies a file larger than <c>500 KB</c>, this method will throw an exception.
	/// </para>
	/// <para>
	/// It is valuable to choose a size limit that corresponds to your use case. If you allow excessively large files, this
	/// may result in excessive consumption of memory or disk/database space, depending on what your code does
	/// with the supplied <see cref="Stream"/>.
	/// </para>
	/// <para>
	/// For Blazor Server in particular, beware of reading the entire stream into a memory buffer unless you have
	/// passed a suitably low size limit, since you will be consuming that memory on the server.
	/// </para>
	/// </param>
	/// <param name="cancellationToken">A cancellation token to signal the cancellation of streaming file data.</param>
	/// <exception cref="IOException">Thrown if the file's length exceeds the <paramref name="maxAllowedSize"/> value.</exception>
	ValueTask<Stream> OpenReadStreamAsync(long maxAllowedSize = 512000, CancellationToken cancellationToken = default);
}