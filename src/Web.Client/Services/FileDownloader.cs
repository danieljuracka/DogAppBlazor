using Microsoft.JSInterop;

namespace DogAppBlazor.Web.Client.Services;

/// <summary>
/// Ponúkne prehliadaču na stiahnutie súbor, ktorý aplikácia vygenerovala (napr. export do Excelu).
/// </summary>
public class FileDownloader(IJSRuntime jsRuntime) : IAsyncDisposable
{
	private const string ModulePath = "./js/file-download.js";

	private readonly IJSRuntime _jsRuntime = jsRuntime;
	private IJSObjectReference _module;

	public async Task DownloadAsync(string fileName, string contentType, byte[] content)
	{
		_module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
		await _module.InvokeVoidAsync("downloadFile", fileName, contentType, content);
	}

	public async ValueTask DisposeAsync()
	{
		if (_module is not null)
		{
			try
			{
				await _module.DisposeAsync();
			}
			catch (JSDisconnectedException)
			{
				// Okruh (Server režim) je už odpojený - modul sa uvoľnil spolu s ním.
			}
		}
		GC.SuppressFinalize(this);
	}
}
