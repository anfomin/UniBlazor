declare global {
	const DotNet: DotNet;

	interface Window {
		readonly DotNet: DotNet;
	}
}

/**
 * Provides methods for invoking .NET methods from JavaScript.
 */
interface DotNet {
	/**
	 * Invokes the specified .NET method synchronously. Recomended to use `invokeMethodAsync` instead.
	 * @param assemblyName The short name (without key/version or .dll extension) of the .NET assembly containing the method.
	 * @param methodId The identifier of the method to invoke. The method must have a `[JSInvokable]` attribute specifying this identifier.
	 * @param args Arguments to pass to the method, each must be JSON-serializable.
	 * @returns The result of the operation.
	 */
	invokeMethod(assemblyName: string, methodId: string, ...args: any[]): any;

	/**
	 * Invokes the specified .NET method asynchronously.
	 * @param assemblyName The short name (without key/version or .dll extension) of the .NET assembly containing the method.
	 * @param methodId The identifier of the method to invoke. The method must have a `[JSInvokable]` attribute specifying this identifier.
	 * @param args Arguments to pass to the method, each must be JSON-serializable.
	 * @returns A promise representing the result of the operation.
	 */
	invokeMethodAsync(assemblyName: string, methodId: string, ...args: any[]): Promise<any>;

	/**
	 * Creates a JavaScript object reference that can be passed to .NET via interop calls.
	 * @param obj The JavaScript object used to create the JavaScript object reference.
	 * @returns The JavaScript object reference (this will be the same instance as the given object).
	 * @throws Error if the given value is not an Object.
	 */
	createJSObjectReference(obj: any): JsObjectReference;

	/**
	 * Creates a JavaScript data reference that can be passed to .NET via interop calls.
	 * @param stream The `ArrayBuffer`, `ArrayBufferView` or `Blob` used to create the JavaScript stream reference.
	 * @returns The JavaScript data reference (this will be the same instance as the given object).
	 * @throws Error if the given value is not an Object or doesn't have a valid byte length.
	 */
	createJSStreamReference(stream: ArrayBuffer | ArrayBufferView | Blob): JsObjectReference;

	/**
	 * Disposes the given JavaScript object reference.
	 * @param obj The JavaScript object reference.
	 */
	disposeJSObjectReference(obj: JsObjectReference): void;
}

/**
 * Represents the .NET instance passed by reference to JavaScript.
 */
export interface DotNetObject {
	/**
	 * Invokes the specified .NET instance method synchronously. Recomended to use `invokeMethodAsync` instead.
	 * @param methodId The identifier of the method to invoke. The method must have a `[JSInvokable]` attribute specifying this identifier.
	 * @param args Arguments to pass to the method, each must be JSON-serializable.
	 * @returns The result of the operation.
	 */
	invokeMethod(methodId: string, ...args: any[]): any;

	/**
	 * Invokes the specified .NET instance method asynchronously.
	 * @param methodId The identifier of the method to invoke. The method must have a `[JSInvokable]` attribute specifying this identifier.
	 * @param args Arguments to pass to the method, each of which must be JSON-serializable.
	 * @returns A promise representing the result of the operation.
	 */
	invokeMethodAsync(methodId: string, ...args: any[]): Promise<any>;

	/**
	 * Dispose the specified .NET instance.
	 */
	dispose(): void;
}

/**
 * Represents the .NET stream passed by reference to JavaScript.
 */
interface DotNetStream {
	/**
	 * Provides stream data.
	 * @returns A promise representing stream `ArrayBuffer`.
	 */
	arrayBuffer(): Promise<ArrayBuffer>;
}

/**
 * Represents the JavaScript object reference.
 */
interface JsObjectReference { }