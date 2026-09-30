using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Web.Client.Components;
using DogAppBlazor.Web.Client.Services;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Pages.Dogs;

public partial class DogDetailPage : ComponentBase
{
	[Inject] protected IDogFacade DogFacade { get; set; }

	[Inject] protected NavigationManager NavigationManager { get; set; }

	[Parameter] public int Id { get; set; }

	private DogDto _dog;
	private bool _notFound;
	private string _color;
	private WeightChartCard _weightChart;

	private string HeroStyle => $"--dog-color: {_color};";

	private string BreedText => String.IsNullOrWhiteSpace(_dog?.Breed) ? "Plemeno neuvedené" : _dog.Breed;

	protected override async Task OnParametersSetAsync()
	{
		_dog = null;
		_notFound = false;

		_dog = await DogFacade.GetDogAsync(Dto.FromValue(Id));
		_notFound = _dog is null;
		_color = DogColors.Normalize(_dog?.Color);
	}

	/// <summary>
	/// Záznam mohol pridať, zmeniť alebo zmazať hmotnosť - graf sa načíta znova.
	/// </summary>
	private Task RefreshWeightChartAsync()
	{
		return _weightChart?.ReloadAsync() ?? Task.CompletedTask;
	}

	private void NavigateToIndex()
	{
		NavigationManager.NavigateTo(NavigationRoutes.Dogs.Index);
	}

	private void NavigateToEdit()
	{
		NavigationManager.NavigateTo(NavigationRoutes.Dogs.GetEdit(Id));
	}
}
