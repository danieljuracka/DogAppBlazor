using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Web.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DogAppBlazor.Web.Client.Components;

public partial class DogCard : ComponentBase
{
	[Inject] protected NavigationManager NavigationManager { get; set; }

	[Parameter, EditorRequired] public DogListItemDto Dog { get; set; }

	/// <summary>
	/// Vyvolá sa klikom na fotku psa (len keď pes fotku má).
	/// </summary>
	[Parameter] public EventCallback<DogListItemDto> OnPhotoClick { get; set; }

	private string _sexText;
	private string _ageText;
	private string _color;
	private string _photoUrl;

	private string BreedText => String.IsNullOrWhiteSpace(Dog.Breed) ? "Plemeno neuvedené" : Dog.Breed;

	private string ColorStyle => $"--dog-color: {_color};";

	protected override void OnParametersSet()
	{
		_sexText = DogFormatter.GetSexText(Dog.Sex);
		_ageText = DogFormatter.GetAgeText(Dog.BirthDate);
		_color = DogColors.Normalize(Dog.Color);
		_photoUrl = NavigationRoutes.PetPhotos.GetPhoto(Dog.PhotoFileName);
	}

	private void NavigateToDetail()
	{
		NavigationManager.NavigateTo(NavigationRoutes.Dogs.GetDetail(Dog.Id));
	}

	private void HandleKeyDown(KeyboardEventArgs e)
	{
		if (e.Key is "Enter")
		{
			NavigateToDetail();
		}
	}

	private Task ShowPhotoAsync()
	{
		return OnPhotoClick.InvokeAsync(Dog);
	}
}
