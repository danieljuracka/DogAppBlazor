using System.Globalization;
using DogAppBlazor.Contracts.Dogs;
using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Web.Client.Components;
using DogAppBlazor.Web.Client.Services;
using DogAppBlazor.Web.Client.Services.DataStores;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Pages.Costs;

public partial class CostsPage : ComponentBase
{
	[Inject] protected IRecordFacade RecordFacade { get; set; }

	[Inject] protected IDogsDataStore DogsDataStore { get; set; }

	[Inject] protected NavigationManager NavigationManager { get; set; }

	[SupplyParameterFromQuery(Name = NavigationRoutes.Costs.DogQueryName)] public int? DogId { get; set; }

	/// <summary>
	/// Rok z URL. Keď chýba, zobrazí sa aktuálny rok, hodnota <see cref="NavigationRoutes.Costs.AllYearsQueryValue"/> znamená celé obdobie.
	/// </summary>
	[SupplyParameterFromQuery(Name = NavigationRoutes.Costs.YearQueryName)] public string YearQuery { get; set; }

	private CostSummaryDto _summary;
	private List<DogListItemDto> _dogs = [];
	private List<int> _availableYears = [];
	private List<CostShareItem> _byType = [];
	private List<CostShareItem> _byDog = [];

	/// <summary>
	/// Zvolený rok. Null znamená celé obdobie.
	/// </summary>
	private int? SelectedYear
	{
		get
		{
			if (String.IsNullOrEmpty(YearQuery))
			{
				return DateTime.Today.Year;
			}

			return Int32.TryParse(YearQuery, NumberStyles.None, CultureInfo.InvariantCulture, out int year) ? year : null;
		}
	}

	private string PeriodText => (SelectedYear is null) ? "za celé obdobie" : $"za rok {SelectedYear}";

	private string ChartTitle => (SelectedYear is null) ? "Podľa rokov" : $"Podľa mesiacov v roku {SelectedYear}";

	/// <summary>
	/// Pes zvolený vo filtri. HxSelect vyžaduje, aby hodnota bola v ponuke, preto neznámy pes z URL znamená „Všetci“.
	/// </summary>
	private int? SelectedDogId => _dogs.Any(d => d.Id == DogId) ? DogId : null;

	protected override async Task OnParametersSetAsync()
	{
		// Stránka sa vykresľuje už počas načítavania - zvolený rok musí byť v ponuke hneď.
		_availableYears = GetAvailableYears(_summary?.AvailableYears ?? []);

		_dogs = await DogsDataStore.GetAllAsync();

		_summary = await RecordFacade.GetCostSummaryAsync(new CostSummaryFilterDto
		{
			DogId = DogId,
			Year = SelectedYear
		});

		_availableYears = GetAvailableYears(_summary.AvailableYears);

		_byType = _summary.ByType
			.Select(item => new CostShareItem(
				RecordTypeFormatter.GetLabel(item.Type),
				RecordTypeFormatter.GetColor(item.Type),
				item.Total,
				item.RecordCount))
			.ToList();

		_byDog = _summary.ByDog
			.Select(item => new CostShareItem(
				item.DogName ?? "Neznámy pes",
				DogColors.Normalize(item.DogColor),
				item.Total,
				item.RecordCount,
				NavigationManager.GetUriWithQueryParameter(NavigationRoutes.Costs.DogQueryName, item.DogId)))
			.ToList();
	}

	/// <summary>
	/// Roky do ponuky. Zvolený rok v nej musí byť, aj keď v ňom nie je žiadny náklad (napr. ručne zadaný v URL).
	/// </summary>
	private List<int> GetAvailableYears(IEnumerable<int> yearsWithCosts)
	{
		return yearsWithCosts
			.Concat((SelectedYear is null) ? [] : [SelectedYear.Value])
			.Distinct()
			.OrderDescending()
			.ToList();
	}

	private void OnYearChanged(int? year)
	{
		string yearQuery = (year is null)
			? NavigationRoutes.Costs.AllYearsQueryValue
			: year.Value.ToString(CultureInfo.InvariantCulture);

		NavigationManager.NavigateTo(NavigationManager.GetUriWithQueryParameter(NavigationRoutes.Costs.YearQueryName, yearQuery));
	}

	private void OnDogChanged(int? dogId)
	{
		NavigationManager.NavigateTo(NavigationManager.GetUriWithQueryParameter(NavigationRoutes.Costs.DogQueryName, dogId));
	}
}
