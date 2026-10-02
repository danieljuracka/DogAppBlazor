using System.Globalization;
using DogAppBlazor.Contracts.Records;
using DogAppBlazor.Web.Client.Services;
using Microsoft.AspNetCore.Components;

namespace DogAppBlazor.Web.Client.Components;

public partial class CostPeriodChart : ComponentBase
{
	[Parameter, EditorRequired] public IReadOnlyList<CostByPeriodDto> Items { get; set; } = [];

	private decimal Max => Items.Count == 0 ? 0 : Items.Max(item => item.Total);

	private string AriaLabel => String.Join(", ", Items.Select(GetTooltip));

	private static string GetLabel(CostByPeriodDto item)
	{
		return (item.Month is null)
			? item.Year.ToString(CultureInfo.InvariantCulture)
			: CostFormatter.GetMonthShortName(item.Month.Value);
	}

	private static string GetTooltip(CostByPeriodDto item)
	{
		string period = (item.Month is null)
			? item.Year.ToString(CultureInfo.InvariantCulture)
			: $"{CostFormatter.GetMonthName(item.Month.Value)} {item.Year}";

		return $"{period}: {CostFormatter.ToMoney(item.Total)}";
	}

	/// <summary>
	/// Zaokrúhlená suma nad stĺpcom - celé eurá, aby sa zmestila aj nad úzky stĺpec.
	/// </summary>
	private static string GetShortAmount(decimal total)
	{
		return Math.Round(total, MidpointRounding.AwayFromZero).ToString("N0") + " €";
	}

	private string GetBarStyle(CostByPeriodDto item)
	{
		decimal percent = (Max == 0) ? 0 : Math.Round(item.Total / Max * 100, 1);
		return $"height: {percent.ToString(CultureInfo.InvariantCulture)}%;";
	}

	private static string GetBarCssClass(CostByPeriodDto item)
	{
		bool isCurrent = (item.Year == DateTime.Today.Year) && ((item.Month is null) || (item.Month == DateTime.Today.Month));
		return isCurrent ? "cost-chart-bar cost-chart-bar-current" : "cost-chart-bar";
	}
}
