import type imask from 'imask';
import type { Masked, FactoryStaticOpts, InputMask } from 'imask';

declare global {
	const IMask: typeof imask;
}
const masks = new WeakMap<HTMLInputElement, InputMask<any>>();

export function create(): Promise<UniMask> {
	if (window['IMask'])
		return Promise.resolve(new UniMask());

	const { promise, resolve } = Promise.withResolvers<UniMask>();
	const script = document.createElement('script');
	script.src = `https://unpkg.com/imask@7.6.1`;
	script.addEventListener('load', () => resolve(new UniMask()));
	document.body.appendChild(script);
	return promise;
}

class UniMask {
	readonly #observer: MutationObserver;

	constructor() {
		for (const input of document.querySelectorAll<HTMLInputElement>('input[data-mask]'))
			this.#applyMask(input);

		this.#observer = new MutationObserver(mutations => {
			const inputs = new Set<HTMLInputElement>();
			for (const mutation of mutations) {
				for (const node of mutation.addedNodes) {
					if (node instanceof HTMLInputElement && node.dataset.mask)
						inputs.add(node);
					else if (node instanceof HTMLElement) {
						for (const input of node.querySelectorAll<HTMLInputElement>('input[data-mask]'))
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
		const maskFormat = input.dataset.mask;
		if (!maskFormat)
			return;
		// if (input.type != 'text') {
		// 	console.warn('Unable to apply mask to input with type not "text"', input);
		// 	return;
		// }

		const options = this.#getMaskOptions(maskFormat);
		if (!options)
			return;

		if (masks.has(input))
			masks.get(input)?.updateOptions(options);
		else {
			const mask = IMask(input, options);
			masks.set(input, mask);

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
				if (hasInputAfterFocus && !mask.typedValue && beforeValue) {
					// console.log('mask clearing value', beforeValue, 'current', input.value);

					// send new value to blazor server
					setTimeout(() => {
						input.value = '';
						input.dispatchEvent(new Event('input', { bubbles: true }));
						input.dispatchEvent(new Event('change', { bubbles: true }));
					}, 50);

					// check value refreshed from blazor server
					let n = 0;
					const interval = setInterval(() => {
						n++;
						if (input.value == beforeValue) {
							input.value = '';
							clearInterval(interval);
						}
						else if (n >= 20)
							clearInterval(interval);
					}, 50);
				}
			});
		}
	}

	#getMaskOptions(maskFormat: string): Masked | FactoryStaticOpts | null {
		switch (maskFormat) {
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
				console.warn('Mask format not supported:', maskFormat);
				return null;
		}
	}
}