import type { DotNetObject } from 'uniblazor';

export function init(element: HTMLElement, server: DotNetObject, multiple: boolean): void {
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
		await sendFiles(server, e.dataTransfer.files, multiple);
	});
}

export function openPicker(server: DotNetObject, accept: string | null, multiple: boolean): void {
	const input = document.createElement('input');
	input.setAttribute('type', 'file');
	if (accept)
		input.setAttribute('accept', accept);
	if (multiple)
		input.setAttribute('multiple', '');
	input.addEventListener('change', async e => {
		if (!input.files?.length)
			return;
		await sendFiles(server, input.files, multiple);
		input.value = '';
	});

	try {
		input.showPicker();
	}
	catch {
		input.click();
	}
}

async function sendFiles(server: DotNetObject, files: FileList, multiple: boolean): Promise<void> {
	const filesToSend = multiple ? Array.from(files) : [files[0]];
	for (const [index, file] of filesToSend.entries()) {
		const stream = DotNet.createJSStreamReference(file);
		if (!await server.invokeMethodAsync('SelectFileAsync', index, files.length,
			file.name, file.lastModified, file.type, stream))
			break;
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