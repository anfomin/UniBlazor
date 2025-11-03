using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace UniBlazor.Internal;

/// <summary>
/// Represents <see cref="ILocalStorage"/> implementation for browser via <see cref="IJSRuntime"/>.
/// </summary>
public class BrowserLocalStorage(IJSRuntime js, IOptions<JsonOptions> jsonOptions)
	: BrowserStorage(js, jsonOptions, "localStorage"), ILocalStorage
{ }