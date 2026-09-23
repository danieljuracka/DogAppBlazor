using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Web.Client.Services.DataStores;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class RecordsSection : ComponentBase
{
	[Inject] protected IRecordFacade RecordFacade { get; set; }

	[Inject] protected IDogsDataStore DogsDataStore { get; set; }

	[Inject] protected IHxMessageBoxService MessageBox { get; set; }

	[Inject] protected IHxMessengerService Messenger { get; set; }

	/// <summary>
	/// Ak je vyplnené, sekcia zobrazuje len záznamy tohto psa a filter sa nepoužíva.
	/// </summary>
	[Parameter] public int? DogId { get; set; }

	[Parameter] public string Title { get; set; } = "Záznamy";

	[Parameter] public bool ShowDogName { get; set; }

	[Parameter] public bool ShowFilter { get; set; }

	[Parameter] public string EmptyTitle { get; set; } = "Zatiaľ žiadne záznamy";

	[Parameter] public string EmptyText { get; set; } = "Pridaj prvý záznam a bude sa mať čo zobrazovať.";

	/// <summary>
	/// Vyvolá sa po každej zmene, aby si rodič mohol obnoviť napríklad pripomienky.
	/// </summary>
	[Parameter] public EventCallback OnChanged { get; set; }

	private RecordEditModal _editModal;
	private RecordDetailModal _detailModal;
	private List<RecordListItemDto> _records;
	private List<DogListItemDto> _dogs = [];
	private readonly RecordFilterDto _filter = new();

	protected override async Task OnParametersSetAsync()
	{
		_filter.DogId = DogId;

		if (ShowFilter)
		{
			_dogs = await DogsDataStore.GetAllAsync();
		}

		await ReloadAsync();
	}

	private async Task ReloadAsync()
	{
		_records = await RecordFacade.GetRecordsAsync(_filter);
	}

	private Task AddAsync()
	{
		return _editModal.ShowNewAsync(DogId);
	}

	private Task ShowDetailAsync(RecordListItemDto record)
	{
		return _detailModal.ShowAsync(record);
	}

	private Task EditAsync(int recordId)
	{
		return _editModal.ShowEditAsync(recordId);
	}

	private Task CompleteFollowUpAsync(int recordId)
	{
		return _editModal.ShowFollowUpAsync(recordId);
	}

	private async Task DeleteAsync(int recordId)
	{
		MessageBoxButtons result = await MessageBox.ShowAsync(new MessageBoxRequest
		{
			Title = "Zmazať záznam",
			Text = "Naozaj chceš tento záznam zmazať? Táto akcia sa nedá vrátiť.",
			Buttons = MessageBoxButtons.YesNo,
			PrimaryButton = MessageBoxButtons.No
		});

		if (result != MessageBoxButtons.Yes)
		{
			return;
		}

		await RecordFacade.DeleteRecordAsync(Dto.FromValue(recordId));
		Messenger.AddInformation("Zmazané", "Záznam je zmazaný.");

		await ReloadAsync();
		await OnChanged.InvokeAsync();
	}

	private async Task OnDogFilterChangedAsync(int? dogId)
	{
		_filter.DogId = dogId;
		await ReloadAsync();
	}

	private async Task OnTypeFilterChangedAsync(RecordTypeEnum? type)
	{
		_filter.Type = type;
		await ReloadAsync();
	}

	private async Task HandleSavedAsync()
	{
		await ReloadAsync();
		await OnChanged.InvokeAsync();
	}
}
