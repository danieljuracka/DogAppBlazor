using System.Globalization;
using DogAppBlazor.Contracts;
using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Web.Client.Services;
using Havit.Blazor.Components.Web.ECharts;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class WeightChartCard : ComponentBase
{
	private const int MinPointsForChart = 2;

	// Tokeny témy - text nikdy nemá farbu série, tú nesie len čiara a body.
	private const string SurfaceColor = "#fffbf7";
	private const string InkColor = "#2e3e44";
	private const string MutedColor = "#75898f";
	private const string GridColor = "#efe6db";

	[Inject] protected IRecordFacade RecordFacade { get; set; }

	[Parameter, EditorRequired] public int DogId { get; set; }

	/// <summary>
	/// Farba psa - farba čiary a bodov grafu.
	/// </summary>
	[Parameter] public string Color { get; set; }

	private List<WeightPointDto> _points;
	private object _chartOptions;
	private int _loadedDogId;

	private WeightPointDto Latest => _points[^1];

	private WeightPointDto Previous => (_points.Count >= 2) ? _points[^2] : null;

	private string ChangeText
	{
		get
		{
			decimal change = Latest.WeightKg - Previous.WeightKg;
			if (change == 0)
			{
				return "bez zmeny";
			}

			string sign = (change > 0) ? "+" : "−";
			return $"{sign}{FormatKg(Math.Abs(change))} kg";
		}
	}

	protected override async Task OnParametersSetAsync()
	{
		if ((_points is null) || (_loadedDogId != DogId))
		{
			await ReloadAsync();
		}
		else
		{
			// Zmenila sa len farba - netreba nové dáta, stačí prekresliť graf.
			BuildChartOptions();
		}
	}

	/// <summary>
	/// Znovu načíta váženia - volá sa po pridaní, úprave alebo zmazaní záznamu.
	/// </summary>
	public async Task ReloadAsync()
	{
		_loadedDogId = DogId;
		_points = await RecordFacade.GetWeightHistoryAsync(Dto.FromValue(DogId));
		BuildChartOptions();
		StateHasChanged();
	}

	private static string FormatKg(decimal value)
	{
		return value.ToString("0.##", CultureInfo.CurrentCulture);
	}

	/// <summary>
	/// Najmenší krok osi X v milisekundách. Do troch mesiacov nechá ECharts značky na dňoch,
	/// pri dlhšom rozsahu sa vynúti mesačný krok.
	/// </summary>
	private double GetMinAxisInterval()
	{
		const double DayInMilliseconds = 24 * 60 * 60 * 1000;

		double spanInDays = (_points[^1].Date - _points[0].Date).TotalDays;
		return (spanInDays > 90) ? (28 * DayInMilliseconds) : 0;
	}

	private void BuildChartOptions()
	{
		if ((_points is null) || (_points.Count < MinPointsForChart))
		{
			_chartOptions = null;
			return;
		}

		string seriesColor = DogColors.Normalize(Color);

		_chartOptions = new
		{
			Animation = true,
			AnimationDuration = 400,
			Grid = new { Left = 8, Right = 16, Top = 16, Bottom = 8, ContainLabel = true },
			Tooltip = new
			{
				Trigger = "axis",
				BackgroundColor = "#fff",
				BorderColor = GridColor,
				Padding = new[] { 8, 12 },
				TextStyle = new { Color = InkColor, FontFamily = "Nunito, system-ui, sans-serif" },
				AxisPointer = new { Type = "line", LineStyle = new { Color = MutedColor, Width = 1 } },
				// Dátum sa skladá z ISO reťazca, nie cez Date, aby ho neposunulo časové pásmo.
				Formatter = new HxEChart.JSFunc(
					"function (params) {\n" +
					"	const p = params[0];\n" +
					"	const [y, m, d] = String(p.value[0]).slice(0, 10).split('-');\n" +
					"	const kg = Number(p.value[1]).toLocaleString('sk-SK', { maximumFractionDigits: 2 });\n" +
					"	return '<div style=\"font-weight:700\">' + kg + ' kg</div>' +\n" +
					$"		'<div style=\"color:{MutedColor}\">' + (+d) + '. ' + (+m) + '. ' + y + '</div>';\n" +
					"}")
			},
			XAxis = new
			{
				Type = "time",
				// Pri dlhšom rozsahu ECharts pridáva aj značky uprostred mesiaca a popisky sa
				// potom miešajú („17. 3.“ vedľa „apr“). Mesačný krok tomu zabráni.
				MinInterval = GetMinAxisInterval(),
				AxisLine = new { LineStyle = new { Color = GridColor } },
				AxisTick = new { Show = false },
				SplitLine = new { Show = false },
				AxisLabel = new
				{
					Color = MutedColor,
					HideOverlap = true,
					// Mesačné značky sú na 1. deň mesiaca: január ukáže rok, ostatné mesiace skratku,
					// pri krátkom rozsahu (denné značky) sa ukáže deň a mesiac.
					Formatter = new HxEChart.JSFunc(
						"function (value) {\n" +
						"	const months = ['jan', 'feb', 'mar', 'apr', 'máj', 'jún', 'júl', 'aug', 'sep', 'okt', 'nov', 'dec'];\n" +
						"	const d = new Date(value);\n" +
						"	if (d.getDate() !== 1) { return d.getDate() + '. ' + (d.getMonth() + 1) + '.'; }\n" +
						"	return (d.getMonth() === 0) ? String(d.getFullYear()) : months[d.getMonth()];\n" +
						"}")
				}
			},
			YAxis = new
			{
				Type = "value",
				// Hmotnosť sa mení o desatiny kg - os od nuly by zmeny zarovnala do rovnej čiary.
				Scale = true,
				SplitNumber = 4,
				SplitLine = new { LineStyle = new { Color = GridColor, Width = 1, Type = "solid" } },
				AxisLabel = new
				{
					Color = MutedColor,
					Formatter = new HxEChart.JSFunc(
						"function (value) { return value.toLocaleString('sk-SK', { maximumFractionDigits: 1 }) + ' kg'; }")
				}
			},
			Series = new object[]
			{
				new
				{
					Name = "Hmotnosť",
					Type = "line",
					Data = _points
						.Select(p => new object[] { p.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), p.WeightKg })
						.ToArray(),
					Symbol = "circle",
					SymbolSize = 9,
					ShowSymbol = true,
					LineStyle = new { Width = 2, Color = seriesColor, Cap = "round", Join = "round" },
					ItemStyle = new { Color = seriesColor, BorderColor = SurfaceColor, BorderWidth = 2 },
					AreaStyle = new { Color = seriesColor, Opacity = 0.10 },
					Emphasis = new { Scale = 1.3 }
				}
			}
		};
	}
}
