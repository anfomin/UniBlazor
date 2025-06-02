export async function scrollToFirstError(formId: string): Promise<boolean> {
	const form = document.getElementById(formId);
	if (!form)
		return false;

	await delay(100);
	const element = form.querySelector('.validation-message')?.parentElement
		|| form.querySelector('.mud-input-error')
		|| form;
	element.scrollIntoView({ behavior: 'smooth' });
	return true;
}

function delay(timeout: number): Promise<void> {
	return new Promise(resolve => setTimeout(resolve, timeout));
}