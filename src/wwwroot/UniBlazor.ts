declare var UniBlazor: any;
window.UniBlazor = {
	scrollIntoView: function (element: HTMLElement, behaviour: ScrollBehavior = 'auto', position: ScrollLogicalPosition = 'start'): void {
		element.scrollIntoView({ behavior: behaviour, block: position });
	},

	downloadFile: async function (fileName: string, stream: DotNetStreamReference): Promise<void> {
		const buffer = await stream.arrayBuffer();
		const blob = new Blob([buffer]);
		const url = URL.createObjectURL(blob);
		try {
			const anchor = document.createElement('a');
			anchor.href = url;
			anchor.download = fileName ?? '';
			anchor.click();
			anchor.remove();
		}
		finally {
			URL.revokeObjectURL(url);
		}
	}
};

interface DotNetStreamReference {
	arrayBuffer(): Promise<ArrayBuffer>;
}