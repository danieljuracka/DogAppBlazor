using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Web.Client.Services.DataStores;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

/// <summary>
/// Modálne okno na pridanie a úpravu záznamu. Otvára sa metódami ShowNewAsync / ShowEditAsync.
/// </summary>
public partial class RecordEditModal : ComponentBase
{
	[Inject] protected IRecordFacade RecordFacade { get; set; }

	[Inject] protected IDogsDataStore DogsDataStore { get; set; }

	[Inject] protected IHxMessengerService Messenger { get; set; }

	/// <summary>
	/// Vyvolá sa po úspešnom uložení, aby volajúci mohol znovu načítať zoznam.
	/// </summary>
	[Parameter] public EventCallback OnSaved { get; set; }

	private HxModal _modal;
	private RecordDto _record;
	private RecordDto _followUpOf;
	private List<DogListItemDto> _dogs = [];
	private string _title;
	private bool _isBusy;

	/// <summary>
	/// Otvorí okno pre nový záznam. DogId predvyplní psa, ku ktorému sa záznam pridáva.
	/// </summary>
	public async Task ShowNewAsync(int? dogId = null)
	{
		await LoadDogsAsync();

		_followUpOf = null;
		_record = new RecordDto
		{
			DogId = dogId ?? _dogs.FirstOrDefault()?.Id ?? 0,
			OccurredOn = DateTime.Today,
			Type = RecordTypeEnum.VetVisit
		};
		_title = "Nový záznam";

		// Bez tohto sa HxModal zobrazí skôr, ako k nemu dorazí nový Title.
		StateHasChanged();
		await _modal.ShowAsync();
	}

	/// <summary>
	/// Otvorí okno s existujúcim záznamom.
	/// </summary>
	public async Task ShowEditAsync(int recordId)
	{
		await LoadDogsAsync();

		_followUpOf = null;
		_record = await RecordFacade.GetRecordAsync(Dto.FromValue(recordId));
		if (_record is null)
		{
			Messenger.AddError("Záznam sa nenašiel", "Pravdepodobne ho medzitým niekto zmazal.");
			return;
		}

		_title = "Úprava záznamu";
		StateHasChanged();
		await _modal.ShowAsync();
	}

	/// <summary>
	/// Otvorí okno pre nový záznam, ktorý vybavuje „ďalší termín“ pôvodného záznamu.
	/// Pes, druh a popis sa prevezmú, dátum je dnes a ďalší termín sa navrhne
	/// s rovnakým odstupom, aký mal pôvodný záznam.
	/// </summary>
	public async Task ShowFollowUpAsync(int originalRecordId)
	{
		await LoadDogsAsync();

		_followUpOf = await RecordFacade.GetRecordAsync(Dto.FromValue(originalRecordId));
		if ((_followUpOf is null) || (_followUpOf.NextDueOn is null))
		{
			Messenger.AddError("Termín sa nenašiel", "Pravdepodobne ho medzitým niekto zmenil alebo zmazal.");
			_followUpOf = null;
			return;
		}

		_record = new RecordDto
		{
			DogId = _followUpOf.DogId,
			Type = _followUpOf.Type,
			Title = _followUpOf.Title,
			VetName = _followUpOf.VetName,
			OccurredOn = DateTime.Today,
			NextDueOn = SuggestNextDueOn(_followUpOf),
			FollowUpOfRecordId = _followUpOf.Id
		};
		_title = "Vybaviť termín";

		StateHasChanged();
		await _modal.ShowAsync();
	}

	public Task HideAsync()
	{
		return _modal.HideAsync();
	}

	/// <summary>
	/// Navrhne ďalší termín s rovnakým odstupom, aký bol medzi dátumom a ďalším termínom
	/// pôvodného záznamu. Ak sa odstup nedá určiť, nenavrhne nič.
	/// </summary>
	private static DateTime? SuggestNextDueOn(RecordDto original)
	{
		if (original.OccurredOn is null)
		{
			return null;
		}

		TimeSpan interval = original.NextDueOn.Value.Date - original.OccurredOn.Value.Date;
		return (interval > TimeSpan.Zero)
			? DateTime.Today.Add(interval)
			: null;
	}

	private async Task LoadDogsAsync()
	{
		_dogs = await DogsDataStore.GetAllAsync();
	}

	private async Task SaveAsync()
	{
		_isBusy = true;
		try
		{
			await RecordFacade.UpdateRecordAsync(_record);
			Messenger.AddInformation("Uložené", "Záznam je uložený.");

			await _modal.HideAsync();
			await OnSaved.InvokeAsync();
		}
		finally
		{
			_isBusy = false;
		}
	}

	private void HandleClosed()
	{
		_record = null;
		_followUpOf = null;
	}
}
