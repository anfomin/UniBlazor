declare global {
	const UniBlazor: UniBlazor;

	interface Window {
		readonly UniBlazor: UniBlazor;
	}
}

/**
 * Provides UniBlazor methods.
 */
export interface UniBlazor {
	/**
	 * Loads JavaScript file. Resolves immediatelly if already loaded.
	 * @param src The script URL.
	 */
	loadScript(src: string): Promise<void>;

	/**
	 * Detects old or not supported browser.
	 * @param min Minimum required browser versions.
	 * @return `true` if the current browser is old or not supported.
	 */
	detectOldBrowser(min?: BrowserList): Promise<boolean>;

	/**
	 * Detects old or not supported browser. If detected adds `<div class="uni-old-browser">` to body with message.
	 * @param opt Options:
	 * - `min` - Minimum required browser versions.
	 * - `text` - Message text. If not specified, a default message in user's language will be used.
	 */
	detectOldBrowserAndShowMessage(opt?: { min?: BrowserList, text?: string | null }): Promise<void>;

	/**
	 * Generates a UUID string.
	 * @param plain If `true`, generates UUID without dashes. Default is `false`.
	 */
	uuid(plain?: boolean): string;
}

export type Browser = 'aol' | 'edge' | 'edge-ios' | 'yandexbrowser' | 'kakaotalk' | 'samsung' | 'silk' | 'miui' | 'beaker' | 'edge-chromium' | 'chrome' | 'chromium-webview' | 'phantomjs' | 'crios' | 'firefox' | 'fxios' | 'opera-mini' | 'opera' | 'pie' | 'netfront' | 'ie' | 'bb10' | 'android' | 'ios' | 'safari' | 'facebook' | 'instagram' | 'ios-webview' | 'curl' | 'searchbot';

export type BrowserList = {
	[key in Browser]?: number | [number, number];
}