import type { BrowserList, DotNetStream } from 'uniblazor';

const loadedScripts = new Map<string, Promise<void>>();
(<any>window).UniBlazor = {
	loadScript: function (src: string): Promise<void> {
		let promise = loadedScripts.get(src);
		if (!promise) {
			promise = new Promise((resolve, reject) => {
				const script = document.createElement('script');
				script.src = src;
				script.addEventListener('load', () => resolve());
				script.addEventListener('error', e => reject(e.message));
				document.body.appendChild(script);
			});
			loadedScripts.set(src, promise);
		}
		return promise;
	},

	detectOldBrowser: async function(min?: BrowserList): Promise<boolean> {
		const minVersions: BrowserList = min || { chrome: 112, firefox: 117, safari: [16, 5], ios: [16, 5], edge: 112, opera: 98, yandexbrowser: [23, 5] };
		const { detect } = await import(<any>'https://unpkg.com/detect-browser@5.3.0/es/index.js');
		const { name, version } = detect();
		const match = /^(\d+)\.(\d+)(\.|$)/.exec(version);
		if (name in minVersions && match) {
			const minVersion = minVersions[name];
			const [minMajor, minMinor] = Array.isArray(minVersion) ? minVersion : [minVersion, 0];
			const major = parseInt(match[1]);
			const minor = parseInt(match[2]);
			if (major < minMajor || major == minMajor && minor < minMinor) {
				console.warn('Detected old browser', name, version, 'min', `${minMajor}.${minMinor}`);
				return true;
			}
		}
		console.debug('Detected supported browser', name, version);
		return false;
	},

	detectOldBrowserAndShowMessage: async function(opt?: { min?: BrowserList, text?: string | null }): Promise<void> {
		const min = opt && opt.min;
		if (await this.detectOldBrowser(min)) {
			const texts = {
				en: 'You use outdated and not supported browser. Please, update your browser to the latest version.',
				ru: 'Вы используете устаревший и неподдерживаемый браузер. Обновите его до последней версии.',
			}
			const text = opt && opt.text || texts[navigator.language.substr(0, 2)] || texts.en;
			const div = document.createElement('div');
			div.classList.add('uni-old-browser');
			div.innerHTML = `<div><img alt="browser-old" src="_content/UniBlazor/browsers.png"/><span>${text}</span></div>`;
			document.body.appendChild(div);
		}
	},

	uuid: function uuid(plain = false): string {
		const res: string = (<any>[1e7]+-1e3+-4e3+-8e3+-1e11).replace(/[018]/g, c =>
			(c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
		);
		return plain ? res.replace(/-/g, '') : res;
	}
};

// handle old browser
const script =  document.currentScript;
const attrBrowser = 'detect-old-browser';
if (script?.hasAttribute(attrBrowser)) {
	const minRaw = script.getAttribute('detect-old-browser');
	const min = minRaw && JSON.parse(minRaw);
	const text = script?.getAttribute('detect-old-browser-text');
	UniBlazor.detectOldBrowserAndShowMessage({ min, text });
}

// disable Blazor scroll to top on navigation
if (script?.hasAttribute('disable-scroll-to-top'))
	window.scrollTo = () => { };