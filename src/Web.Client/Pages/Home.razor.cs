using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Web.Client.Components;
using DogAppBlazor.Web.Client.Services.DataStores;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Pages;

public partial class Home : ComponentBase
{
	/// <summary>
	/// Počet zástupných kariet počas načítavania zoznamu.
	/// </summary>
	private const int SkeletonCount = 2;

	[Inject] protected IDogsDataStore DogsDataStore { get; set; }

	[Inject] protected IRecordFacade RecordFacade { get; set; }

	[Inject] protected NavigationManager NavigationManager { get; set; }

	private List<DogListItemDto> _dogs;
	private List<ReminderDto> _reminders;
	private RecordEditModal _recordEditModal;
	private string _subtitle = "Načítavam…";

	protected override async Task OnInitializedAsync()
	{
		_dogs = await DogsDataStore.GetAllAsync();
		_subtitle = BuildSubtitle();

		await LoadRemindersAsync();
	}

	private async Task LoadRemindersAsync()
	{
		_reminders = await RecordFacade.GetUpcomingRemindersAsync();
	}

	private Task CompleteReminderAsync(int recordId)
	{
		return _recordEditModal.ShowFollowUpAsync(recordId);
	}

	private void NavigateToCreate()
	{
		NavigationManager.NavigateTo(NavigationRoutes.Dogs.Create);
	}

	private string BuildSubtitle()
	{
		if (_dogs.Count == 0)
		{
			return "Zatiaľ bez psov";
		}

		string[] names = _dogs.ConvertAll(d => d.Name).ToArray();
		string list = (names.Length == 1)
			? names[0]
			: String.Join(", ", names[..^1]) + " a " + names[^1];

		return $"Staráme sa o {list}";
	}
}
