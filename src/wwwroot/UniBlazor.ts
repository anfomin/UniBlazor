declare var UniBlazor: any;
window.UniBlazor = {
	scrollIntoView: function (element: HTMLElement, behaviour: ScrollBehavior = 'auto', position: ScrollLogicalPosition = 'start') {
		element.scrollIntoView({ behavior: behaviour, block: position });
	}
};