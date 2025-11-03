import type { DotNetStream } from 'uniblazor';

export function initTimeZone(): string {
	const timezone = Intl.DateTimeFormat().resolvedOptions().timeZone;
	setCookie('uni-timezone', timezone, { maxAge: 60*60*24*365, sameSite: 'lax' });
	return timezone;
}

export function setCookie(key: string, value: string, opt?: {
	domain?: string;
	path?: string;
	expires?: string;
	maxAge?: number;
	sameSite?: 'lax' | 'strict' | 'none';
	secure?: boolean;
}): void {
	let cookie = `${key}=${value}`;
	if (opt?.domain)
		cookie += `;domain=${opt.domain}`;
	if (opt?.path)
		cookie += `;path=${opt.path}`;
	if (opt?.expires)
		cookie += `;expires=${opt.expires}`;
	if (opt?.maxAge != null)
		cookie += `;max-age=${opt.maxAge}`;
	if (opt?.sameSite)
		cookie += `;samesite=${opt.sameSite}`;
	if (opt?.secure)
		cookie += `;secure`;
	document.cookie = cookie;
}

export function scrollIntoView(element: HTMLElement, behaviour: ScrollBehavior = 'auto', position: ScrollLogicalPosition = 'start'): void {
	element.scrollIntoView({ behavior: behaviour, block: position });
}

export async function scrollToFirstError(formId: string): Promise<boolean> {
	const form = document.getElementById(formId);
	if (!form)
		return false;

	await delay(100);
	const element = form.querySelector('.validation-message')?.parentElement
		|| form.querySelector('[aria-invalid=true]')
		|| form;
	element.scrollIntoView({ behavior: 'smooth' });
	return true;
}

export async function downloadFile(fileName: string, stream: DotNetStream): Promise<void> {
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

function delay(timeout: number): Promise<void> {
	return new Promise(resolve => setTimeout(resolve, timeout));
}