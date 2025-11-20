import type { DotNetObject } from 'uniblazor';

export function init(element: HTMLElement, server: DotNetObject): void {
	const clsDragging = 'dragging';
	if (!element)
		return;

	element.addEventListener('dragenter', e => {
		element.classList.add(clsDragging);
	});

	element.addEventListener('dragleave', e => {
		if (e.relatedTarget instanceof Node && isChild(e.relatedTarget, element))
			return;
		element.classList.remove(clsDragging);
	});

	element.addEventListener('dragover', e => {
		if (e.dataTransfer?.types.includes('Files'))
			e.preventDefault();
	});

	element.addEventListener('drop', async e => {
		element.classList.remove(clsDragging);
		if (e.dataTransfer?.types.includes('Files') !== true || !e.dataTransfer.files?.length)
			return;

		e.preventDefault();
		const file = e.dataTransfer.files[0];
		const stream = DotNet.createJSStreamReference(file);
		await server.invokeMethodAsync('SelectFileAsync', file.name, file.lastModified, file.type, stream);
	});
}

export function openPicker(server: DotNetObject, accept: string | null): void {
	const input = document.createElement('input');
	input.setAttribute('type', 'file');
	if (accept)
		input.setAttribute('accept', accept);
	input.addEventListener('change', async e => {
		if (!input.files?.length)
			return;

		const file = input.files[0];
		const stream = DotNet.createJSStreamReference(file);
		await server.invokeMethodAsync('SelectFileAsync', file.name, file.lastModified, file.type, stream);
		input.value = '';
	});

	try {
		input.showPicker();
	}
	catch {
		input.click();
	}
}

function isChild(element: Node, root: Node): boolean {
	let parent: Node | null = element;
	while (parent) {
		if (parent == root)
			return true;
		parent = parent.parentNode;
	}
	return false;
}