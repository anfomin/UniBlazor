declare var DotNet: DotNetGlobal;

interface DotNetGlobal {
	invokeMethodAsync(assembly: string, methodName: string, ...args: any[]): Promise<any>;
	invokeMethod(assembly: string, methodName: string, ...args: any[]): any;
	createJSStreamReference(stream: ArrayBuffer | Blob): JsStreamReference;
}

interface DotNetObjectReference {
	invokeMethodAsync(methodName: string, ...args: any[]): Promise<any>;
	invokeMethod(methodName: string, ...args: any[]): any;
}

interface JsStreamReference { }

interface DotNetStreamReference {
	arrayBuffer(): Promise<ArrayBuffer>;
}