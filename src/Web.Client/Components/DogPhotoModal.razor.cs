using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

/// <summary>
/// Popup so zväčšenou fotkou psa. Otvára sa klikom na fotku na karte psa.
/// </summary>
public partial class DogPhotoModal : ComponentBase
{
	private HxModal _modal;
	private string _dogName;
	private string _photoUrl;

	public async Task ShowAsync(string dogName, string photoUrl)
	{
		_dogName = dogName;
		_photoUrl = photoUrl;

		StateHasChanged();
		await _modal.ShowAsync();
	}

	private void HandleClosed()
	{
		_photoUrl = null;
	}
}
