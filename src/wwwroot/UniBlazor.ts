declare var UniBlazor: any;
declare type Browser = 'aol' | 'edge' | 'edge-ios' | 'yandexbrowser' | 'kakaotalk' | 'samsung' | 'silk' | 'miui' | 'beaker' | 'edge-chromium' | 'chrome' | 'chromium-webview' | 'phantomjs' | 'crios' | 'firefox' | 'fxios' | 'opera-mini' | 'opera' | 'pie' | 'netfront' | 'ie' | 'bb10' | 'android' | 'ios' | 'safari' | 'facebook' | 'instagram' | 'ios-webview' | 'curl' | 'searchbot';
declare type BrowserList = {
	[key in Browser]?: number | [number, number];
}
const loadedScripts = new Map<string, Promise<void>>();
window.UniBlazor = {
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
		const minVersions: BrowserList = min || { chrome: 119, firefox: 121, safari: [17, 4], ios: [17, 4], edge: 119, opera: 105, yandexbrowser: [23, 9] };
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

	detectOldBrowserAndShowMessage: async function(opt?: { min?: BrowserList, text?: string }): Promise<void> {
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

	getTimeZone: function(): string {
		return Intl.DateTimeFormat().resolvedOptions().timeZone;
	},

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

const script =  document.currentScript;
const attrBrowser = 'detect-old-browser';
if (script?.hasAttribute(attrBrowser)) {
	const minRaw = script.getAttribute('detect-old-browser');
	const min = minRaw && JSON.parse(minRaw);
	const text = script?.getAttribute('detect-old-browser-text');
	UniBlazor.detectOldBrowserAndShowMessage({ min, text });
}
if (script?.hasAttribute('disable-scroll-to-top'))
	window.scrollTo = () => { }; // disable Blazor scroll to top on navigation