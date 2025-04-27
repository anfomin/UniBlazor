import type imask from 'imask';
import type { Masked, FactoryStaticOpts, InputMask } from 'imask';

declare global {
	const IMask: typeof imask;
}
const masks = new WeakMap<HTMLInputElement, InputMask<any>>();

export async function create(): Promise<UniMask> {
	if (!window['IMask'])
		await UniBlazor.loadScript('https://unpkg.com/imask@7.6.1');
	return new UniMask();
}

class UniMask {
	readonly #observer: MutationObserver;
	readonly #isSafari = /^((?!chrome|android).)*safari/i.test(navigator.userAgent);

	constructor() {
		const selector = 'input[data-mask],input[data-mask-regexp]';
		for (const input of document.querySelectorAll<HTMLInputElement>(selector))
			this.#applyMask(input);

		this.#observer = new MutationObserver(mutations => {
			const inputs = new Set<HTMLInputElement>();
			for (const mutation of mutations) {
				for (const node of mutation.addedNodes) {
					if (node instanceof HTMLInputElement && node.dataset.mask)
						inputs.add(node);
					else if (node instanceof HTMLElement) {
						for (const input of node.querySelectorAll<HTMLInputElement>(selector))
							inputs.add(input);
					}
				}
			}
			for (const input of inputs)
				this.#applyMask(input);
		});
		this.#observer.observe(document.body, { childList: true, subtree: true });
	}

	destroy(): void {
		this.#observer.disconnect();
	}

	#applyMask(input: HTMLInputElement): void {
		const maskType = input.dataset.mask;
		const maskRegexp = input.dataset.maskRegexp;
		const options = maskType ? this.#getMaskOptionsForType(maskType)
			: maskRegexp ? { mask: new RegExp(maskRegexp) }
			: null;
		if (!options)
			return;
		// if (input.type != 'text') {
		// 	console.warn('Unable to apply mask to input with type not "text"', input);
		// 	return;
		// }

		if (masks.has(input))
			masks.get(input)?.updateOptions(options);
		else {
			const mask = IMask(input, options);
			masks.set(input, mask);

			// setting lazy=false on Safari breaks the input
			if (!this.#isSafari)
				this.#enableLazyOnFocus(input, mask);
		}
	}

	#getMaskOptionsForType(type: string): Masked<any> | FactoryStaticOpts | null {
		switch (type) {
			case 'digits':
				return { mask: /^\d+$/ };
			case 'date':
				return new IMask.MaskedDate({ overwrite: true });
			case 'time':
			case 'timeShort':
				return {
					mask: 'hh:`mm',
					overwrite: true,
					blocks: {
						hh: { mask: IMask.MaskedRange, from: 0, to: 23 },
						mm: { mask: IMask.MaskedRange, from: 0, to: 59 }
					}
				};
			case 'timeLong':
				return {
					mask: 'hh:`mm:`ss',
					overwrite: true,
					blocks: {
						hh: { mask: IMask.MaskedRange, from: 0, to: 23 },
						mm: { mask: IMask.MaskedRange, from: 0, to: 59 },
						ss: { mask: IMask.MaskedRange, from: 0, to: 59 }
					}
				};
			case 'duration':
				return {
					mask: 'mm:`ss',
					overwrite: true,
					blocks: {
						mm: { mask: IMask.MaskedRange, from: 0, to: 59 },
						ss: { mask: IMask.MaskedRange, from: 0, to: 59 }
					}
				};
			case 'phone':
				return {
					mask: '+{7} 000 000-00-00'
				}
			default:
				console.warn(`Mask type '${type}' is not supported.`, 'Supported types are: digits, date, time, timeShort, timeLong, duration, phone.');
				return null;
		}
	}

	#enableLazyOnFocus(input: HTMLInputElement, mask: InputMask<Masked<any> | FactoryStaticOpts>): void {
		let hasInputAfterFocus = false;
		input.addEventListener('focus', () => {
			// console.log('focus', 'input value', input.value, 'mask value', mask.value);
			mask.updateValue();
			mask.updateOptions({ lazy: false });
			hasInputAfterFocus = false;
		});
		input.addEventListener('input', () => hasInputAfterFocus = true);
		input.addEventListener('blur', () => {
			// console.log('blur', 'input value', input.value, 'mask value', mask.value, 'typed', mask.typedValue, 'unmasked', mask.unmaskedValue);
			const beforeValue = input.value;
			mask.updateOptions({ lazy: true });
			// console.log('blur after lazy', 'input value', input.value, 'mask value', mask.value, 'typed', mask.typedValue, 'unmasked', mask.unmaskedValue);
			if (hasInputAfterFocus && !mask.typedValue && beforeValue) {
				// console.log('mask clearing value', beforeValue, 'current', input.value);
				// send new value to blazor server
				input.value = '';
				input.dispatchEvent(new Event('input', { bubbles: true }));
				input.dispatchEvent(new Event('change', { bubbles: true }));
			}
		});
	}
}