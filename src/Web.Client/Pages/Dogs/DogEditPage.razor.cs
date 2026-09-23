using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Web.Client.Services;
using DogAppBlazor.Web.Client.Services.DataStores;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Pages.Dogs;

public partial class DogEditPage : ComponentBase
{
	[Inject] protected IDogFacade DogFacade { get; set; }

	[Inject] protected IDogsDataStore DogsDataStore { get; set; }

	[Inject] protected IHxMessengerService Messenger { get; set; }

	[Inject] protected NavigationManager NavigationManager { get; set; }

	/// <summary>
	/// Pri zakladaní nového psa je null.
	/// </summary>
	[Parameter] public int? Id { get; set; }

	private DogDto _dog;
	private bool _notFound;
	private bool _isBusy;
	private string _title;
	private string _subtitle;

	private bool IsNew => Id is null;

	protected override async Task OnParametersSetAsync()
	{
		_title = IsNew ? "Nový pes" : "Úprava profilu";
		_subtitle = IsNew
			? "Vyplň, čo o ňom vieš. Doplniť sa dá kedykoľvek neskôr."
			: "Zmeny sa uložia až po kliknutí na Uložiť.";

		_dog = null;
		_notFound = false;

		if (IsNew)
		{
			List<DogListItemDto> existingDogs = await DogsDataStore.GetAllAsync();
			_dog = new DogDto
			{
				Color = DogColors.SuggestUnused(existingDogs.Select(d => d.Color))
			};
			return;
		}

		_dog = await DogFacade.GetDogAsync(Dto.FromValue(Id.Value));
		_notFound = _dog is null;
	}

	private async Task SaveAsync()
	{
		_isBusy = true;
		try
		{
			Dto<int> savedId = await DogFacade.UpdateDogAsync(_dog);

			// Zoznam psov je v klientskej cache - po zmene ju treba zahodiť.
			DogsDataStore.Clear();

			Messenger.AddInformation("Uložené", $"Profil psa {_dog.Name} je uložený.");
			NavigationManager.NavigateTo(NavigationRoutes.Dogs.GetDetail(savedId.Value));
		}
		finally
		{
			_isBusy = false;
		}
	}

	private void Cancel()
	{
		NavigationManager.NavigateTo(IsNew
			? NavigationRoutes.Dogs.Index
			: NavigationRoutes.Dogs.GetDetail(Id.Value));
	}
}
