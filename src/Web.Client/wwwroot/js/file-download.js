// Stiahnutie súboru vygenerovaného v aplikácii (napr. export do Excelu).
// Obsah prichádza z .NET ako Uint8Array.

export function downloadFile(fileName, contentType, content) {
	const blob = new Blob([content], { type: contentType });
	const url = URL.createObjectURL(blob);

	const link = document.createElement('a');
	link.href = url;
	link.download = fileName;
	document.body.appendChild(link);
	link.click();
	link.remove();

	// Uvoľnenie až po spracovaní kliknutia - niektoré prehliadače inak sťahovanie zrušia.
	setTimeout(() => URL.revokeObjectURL(url), 1000);
}
