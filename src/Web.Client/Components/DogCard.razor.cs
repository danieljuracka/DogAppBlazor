using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Web.Client.Services;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class DogCard : ComponentBase
{
	[Inject] protected NavigationManager NavigationManager { get; set; }

	[Parameter, EditorRequired] public DogListItemDto Dog { get; set; }

	private string _sexText;
	private string _ageText;
	private string _color;

	private string BreedText => String.IsNullOrWhiteSpace(Dog.Breed) ? "Plemeno neuvedené" : Dog.Breed;

	private string ColorStyle => $"--dog-color: {_color};";

	protected override void OnParametersSet()
	{
		_sexText = DogFormatter.GetSexText(Dog.Sex);
		_ageText = DogFormatter.GetAgeText(Dog.BirthDate);
		_color = DogColors.Normalize(Dog.Color);
	}

	private void NavigateToDetail()
	{
		NavigationManager.NavigateTo(NavigationRoutes.Dogs.GetDetail(Dog.Id));
	}

	private void NavigateToEdit()
	{
		NavigationManager.NavigateTo(NavigationRoutes.Dogs.GetEdit(Dog.Id));
	}
}
